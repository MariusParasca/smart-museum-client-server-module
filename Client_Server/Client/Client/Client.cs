using System;
using System.Text;
using System.IO;
using System.Net.Sockets;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;

class Constants
{
    public const int type_length = 8;
    public const int data_length = 1016;
}
[StructLayout(LayoutKind.Sequential, Size = Constants.type_length + Constants.type_length)]
internal struct Packet
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Constants.type_length)]
    public string type;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.type_length)]
    public byte[] data;
}
namespace Client
{
     
    public class Client
    {
        
        private static Exhibit exhibit;
        private static Compresser Compresser;
        private static BinaryReader binaryReader;
        private static BinaryWriter binaryWriter;
        private static TcpClient tcpclnt;
        private static Packet endPacket;
        public static void Main()
        {
            Compresser = new Compresser();
     //       exhibit = new Exhibit();
            try
            {

                TcpClient tcpclnt = new TcpClient();
                endPacket.type = "[EndT]";
                endPacket.data = new byte[1];
                Console.WriteLine("Connecting.....");
                tcpclnt.Connect("127.0.0.1", 8001);
                Console.WriteLine("Connected");
                binaryWriter = new BinaryWriter(tcpclnt.GetStream());
                
                binaryReader = new BinaryReader(tcpclnt.GetStream());

                //SendText("String de test");
                //ReceivePhoto("test.jpg");
                //ReceiveText();
                // SendPhoto( ".//Resources//test.jpg");
                ReceiveZip();
                Console.WriteLine("\nJob done! Now exit!");
                tcpclnt.Close();
            }

            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.StackTrace);
            }

        }

        public static BinaryReader GetBinaryReader() { return binaryReader; }
        public static BinaryWriter GetBinaryWriter() { return binaryWriter; }

        public static void connectToServer(String ip, int port)
        {
            tcpclnt = new TcpClient();

            Console.WriteLine("Connecting.....");
            tcpclnt.Connect(ip, port);
            Console.WriteLine("Connected");
            binaryWriter = new BinaryWriter(tcpclnt.GetStream());
            binaryReader = new BinaryReader(tcpclnt.GetStream());
        }

        public static string ReceiveText()
        {
            byte[] data = Receive();
            return Encoding.ASCII.GetString(data, 0, data.Length);
        }
        private static Packet bytesToPacket(byte[] arr)
        {
            try
            {
                Packet packet = new Packet();
                byte[] tarr = new byte[Constants.type_length];
                Array.Copy(arr, tarr, Constants.type_length);
                packet.type = Encoding.UTF8.GetString(tarr);
                packet.data = new byte[arr.Length - Constants.type_length];
                Array.Copy(arr, Constants.type_length, packet.data, 0, arr.Length - Constants.type_length);
                return packet;  
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Packet packet = new Packet();
                packet.type = "[Error]";
                return packet;
            }
        }
        public static void ReceivePhoto(String fileName)
        {
            byte[] data = Receive();
            using (var ms = new MemoryStream(data))
            {
                Image.FromStream(ms).Save(".\\Resources\\" + fileName);
                Console.WriteLine("[PHOTO] Received \n");
            }
        }
        internal static byte[] Receive()
        {

            try
            {
                Packet packet = new Packet();
                int len = binaryReader.ReadInt32();
                packet.data = new byte[Constants.data_length];
                int cnt = 0;
                byte[] data = new byte[len + Constants.data_length];
                byte[] packetBytes = new byte[Constants.data_length + Constants.type_length];
                while (cnt < len)
                {
                    int howBig = binaryReader.ReadInt32();
                  
                    int readed = binaryReader.Read(packetBytes, 0, howBig);
                    int myCheckSum = CalculateChecksum(packetBytes);
                    int checkSum = binaryReader.ReadInt32();
                    if (myCheckSum != checkSum)
                    {
                        packet.type = "[Error]";
                        packet.data = Encoding.ASCII.GetBytes("Checksum does not match!" + myCheckSum + " " + checkSum);
                        Console.WriteLine("[" + DateTime.Now + "] [Error] Checksum does not match!");
                        return null;
                    }
                    packet = bytesToPacket(packetBytes);
                    Array.Copy(packetBytes, Constants.type_length, data, cnt, howBig - Constants.type_length);
                    cnt += howBig - Constants.type_length;
                  
                    if (packet.type == "[EndT]")
                        break;

                    Console.WriteLine("[" + DateTime.Now + "] Packet received!");
                }
                return data;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;

            }

        }
        internal static byte[] ReceiveZip()
        {

            try
            {
                Packet packet = new Packet();
                int len = binaryReader.ReadInt32();
                //hh:mm:ss
                packet.data = new byte[Constants.data_length];
                int cnt = 0;
                byte[] data = new byte[len + Constants.data_length];
                byte[] packetBytes = new byte[Constants.data_length + Constants.type_length];
                DateTime dateTime = DateTime.Now;
                //FileStream fs = File.Create( );
                string filename = ".//Resources//" + dateTime.ToString("dd_MM_yyyy_hh_mm_ss") + ".zip";
                Stream fs = new FileStream(filename, FileMode.Append);
                BinaryWriter bw = new BinaryWriter(fs);

                while (cnt < len)
                {
                    int howBig = binaryReader.ReadInt32();

                    int readed = binaryReader.Read(packetBytes, 0, howBig);
                    int myCheckSum = CalculateChecksum(packetBytes);
                    int checkSum = binaryReader.ReadInt32();
                    if (myCheckSum != checkSum)
                    {
                        packet.type = "[Error]";
                        packet.data = Encoding.ASCII.GetBytes("Checksum does not match!" + myCheckSum + " " + checkSum);
                        Console.WriteLine("[" + DateTime.Now + "] [Error] Checksum does not match!");
                        return null;
                    }
                    packet = bytesToPacket(packetBytes);
                    cnt += howBig - Constants.type_length;

                    bw.Write(packet.data);
                    bw.Flush();
                    Console.WriteLine("[" + DateTime.Now + "] Packet received!");
                }
                return data;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;

            }

        }
        private static void SendZip(Socket socket, string type, string filePath)
        {
            try
            {
                Packet packet = new Packet();
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                byte[] packetBytes = new byte[Constants.data_length + Constants.type_length];
                byte[] data = new byte[Constants.data_length];
                packet.data = new byte[Constants.data_length];
                int cnt = 0;
                int x = 0;
                int size = 0, checkSum = 0;
                int len = (int)new FileInfo(filePath).Length;
                FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                socket.Send(BitConverter.GetBytes(len), 4, SocketFlags.None);
                while (cnt < len)
                {
                    x = Math.Min(Constants.data_length, len - cnt);
                    packet.type = type;
                    int read = fs.Read(data, 0, Constants.data_length);
                    Array.Copy(data, 0, packet.data, 0, x);
                    packetBytes = packetToBytes(packet);
                    size = packetBytes.Length;
                    socket.Send(BitConverter.GetBytes(size), 4, SocketFlags.None);
                    socket.Send(packetBytes, size, SocketFlags.None);
                    Console.WriteLine("[" + DateTime.Now + "] Packet sent! ");
                    checkSum = CalculateChecksum(packetBytes);
                    socket.Send(BitConverter.GetBytes(checkSum), 4, SocketFlags.None);
                    cnt += x;
                }


            }

            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.StackTrace);
            }
        }

        private static int CalculateChecksum(byte[] packetBytes)
        {
            int checksum = 0;
            foreach (byte chData in packetBytes)
                checksum += chData;
            return checksum;
        }


        public static void SendText(String text)
        {
            Send("[Text]",  Encoding.UTF8.GetBytes(text));
            Console.WriteLine("[" + DateTime.Now + "] Text Sent!");
        }

        public static void SendPhoto(String imagePath)
        {
            Bitmap bitmap = new Bitmap(imagePath);
            byte[] imageBytes = ImageToByteArray(bitmap);
            Send("[Image]", imageBytes);
            Console.WriteLine("[" + DateTime.Now + "] Image Sent!");
        }

        internal static byte[] packetToBytes(Packet packet)
        {
            try
            {

                byte[] packetBytes = Encoding.UTF8.GetBytes(packet.type);
                Array.Resize<byte>(ref packetBytes, 8 + packet.data.Length);
                Array.Copy(packet.data, 0, packetBytes, 8, packet.data.Length);
                return packetBytes;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.StackTrace);
                return null;
            }
        }
        private static void Send(string type, byte[] data)
        {
            try
            {
                Packet packet = new Packet();
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                byte[] packetBytes = new byte[Constants.data_length + Constants.type_length];
                packet.data = new byte[Constants.data_length];
                int cnt = 0;
                int x = 0;
                int size = 0, checkSum = 0;
                int len = data.Length;
                binaryWriter.Write(BitConverter.GetBytes(len), 0, 4);
                while (cnt < len)
                {
                    x = Math.Min(Constants.data_length, len - cnt);
                    packet.type = type;
                    Array.Copy(data, cnt, packet.data, 0, x);
                    packetBytes = packetToBytes(packet);
                    size = packetBytes.Length;
                    binaryWriter.Write(BitConverter.GetBytes(size), 0, 4);
                    binaryWriter.Write(packetBytes, 0, size);
                    Console.WriteLine("[" + DateTime.Now + "] Packet sent! ");
                     checkSum = CalculateChecksum(packetBytes);
                    Console.WriteLine(checkSum);
                    binaryWriter.Write(BitConverter.GetBytes(checkSum), 0, 4);
                    binaryWriter.Flush();
                    cnt += x;
                }
              

            }

            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.StackTrace);
            }
        }

        private static byte[] ImageToByteArray(Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, imageIn.RawFormat);
                return ms.ToArray();
            }
        }

       
    }



}
