using System;
using System.Text;
using System.IO;
using System.Net.Sockets;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;

[StructLayout(LayoutKind.Sequential, Size = 1024)]
//[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
internal struct Packet
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
    public string type;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1016)]
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

        public static void Main()
        {
            Compresser = new Compresser();
     //       exhibit = new Exhibit();
            try
            {

                TcpClient tcpclnt = new TcpClient();

                Console.WriteLine("Connecting.....");
                tcpclnt.Connect("127.0.0.1", 8001);
                Console.WriteLine("Connected");
                binaryWriter = new BinaryWriter(tcpclnt.GetStream());
                
                binaryReader = new BinaryReader(tcpclnt.GetStream());


                //trimitere text
                SendText("String de test");
                //   ReceivePhoto("test.jpg");


                //primire text
                //  Console.WriteLine(ReceiveText());

                //        Console.WriteLine("byte array file recevied");


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

    public static String ReceiveText()
        {
            Packet packet = Receive();
            return Encoding.ASCII.GetString(packet.data, 0, packet.data.Length);
        }
        private static Packet bytesToPacket(byte[] arr)
        {
            try
            {
                int rawsize = Marshal.SizeOf(typeof(Packet));
                if (rawsize > arr.Length)
                    return default(Packet);

                IntPtr buffer = Marshal.AllocHGlobal(rawsize);
                Marshal.Copy(arr, 0, buffer, rawsize);
                Packet obj = (Packet)Marshal.PtrToStructure(buffer, typeof(Packet));
                Marshal.FreeHGlobal(buffer);
                return obj;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Packet packet = new Packet();
                packet.type = "[Error]";//Encoding.ASCII.GetBytes("[Error]");
                return packet;
            }
        }
        public static void ReceivePhoto(String fileName)
        {
            Packet packet = Receive();
            using (var ms = new MemoryStream(packet.data))
            {
                Image.FromStream(ms).Save(fileName);
                Console.WriteLine("[PHOTO] Received \n");
            }
        }
        internal static Packet Receive()
        {
            try
            {
                Packet packet;
                int howBig = binaryReader.ReadInt32();
                byte[] packetBytes = new byte[howBig];
                int readed = binaryReader.Read(packetBytes, 0, howBig);
                int myCheckSum = CalculateChecksum(packetBytes);
                int checkSum = binaryReader.ReadInt32();
                if (myCheckSum != checkSum)
                {
                    packet.type = "[Error]";// Encoding.ASCII.GetBytes("[Error]");
                    packet.data = Encoding.ASCII.GetBytes("Checksum does not match!");
                    Console.WriteLine("[" + DateTime.Now + "] [Error] Checksum does not match!");
                    return packet;
                }
                byte[] decompressedByteArray = Compresser.Decompress(packetBytes);
                packet = bytesToPacket(decompressedByteArray);

                Console.WriteLine("[" + DateTime.Now + "] Packet received!");
                return packet;
            }
            catch (Exception e)
            {

                Console.WriteLine("Error..... " + e.StackTrace);
                return new Packet();
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
            // ASCIIEncoding asen = new ASCIIEncoding();
            Packet packet;
            packet.type = "[Text]";// Encoding.ASCII.GetBytes("[Text]");
            Console.WriteLine(text);
            //     Console.WriteLine(Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(text)));
            packet.data = Encoding.UTF8.GetBytes(text);
            //   Console.WriteLine(Encoding.ASCII.GetString(packet.data));

            Send(packet);
            Console.WriteLine("[" + DateTime.Now + "] Text Sent!");
        }

        public static void SendPhoto(String imagePath)
        {
            Bitmap bitmap = new Bitmap(imagePath);
            byte[] imageByte = ImageToByteArray(bitmap);
            Packet packet;
            packet.type = "[Image]";// Encoding.ASCII.GetBytes("[Image]");
            packet.data = imageByte;
            Send(packet);
            Console.WriteLine("[" + DateTime.Now + "] Image Sent!");
        }

        internal static byte[] packetToBytes(Packet packet)
        {
            try
            {

                int size = 1024;
                IntPtr buffer = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(  packet, buffer, false);
                byte[] rawData = new byte[size];
                Marshal.Copy(buffer, rawData, 0, size);
                Marshal.FreeHGlobal(buffer);
                return rawData;
                // var formatter = new BinaryFormatter();
                //  formatter.Serialize()
                // return arr;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.StackTrace);
                return null;
            }
        }
        private static void Send(Packet packet)
        {
            try
            {
                //   binaryWriter.Flush();
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                byte[] packetBytes = Encoding.UTF8.GetBytes(packet.type);
                Array.Resize<byte>(ref packetBytes, 8 + packet.data.Length);
                Array.Copy(packet.data,0, packetBytes, 8, packet.data.Length);
           //     packetBytes = Compresser.Compress(packetBytes);
                int size = packetBytes.Length;
                Console.Write("* *" + size);
               
                //if (!BitConverter.IsLittleEndian)
                  //  Array.Reverse(intBytes);
                binaryWriter.Write(BitConverter.GetBytes(size), 0, 4);
                binaryWriter.Flush();
                binaryWriter.Write(packetBytes, 0, size);
                binaryWriter.Flush();
                Console.WriteLine("[" + DateTime.Now + "] Packet sent! ");
                int checkSum = CalculateChecksum(packetBytes);
                Console.WriteLine(checkSum);
                binaryWriter.Write(BitConverter.GetBytes(checkSum), 0, 4);
                binaryWriter.Flush();
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
