using System;
using System.Text;
using System.IO;
using System.Net.Sockets;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;
using System.Text.RegularExpressions;

class Constants
{
    public const int type_length = 50;
    public const int data_length = 974;
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
            //Museum museum2 = new Museum(".\\Resources\\muzeu_de_test");
            //Exhibit exhibit2 = new Exhibit(".\\Resources\\muzeu_de_test\\Tablou_de_test");

            GetPacketNameFromPacketType("[Museum]-muzeu_de_test");
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
                //ReceiveZip();

                //SendText("String de test");
                //ReceivePhoto("test.jpg");
                //ReceiveText();
                //SendPhoto( ".//Resources//test.jpg");
                
                //Teste
                //Museum museum = new Museum(binaryWriter, binaryReader, "Muzeu de test");
                //museum = new Museum(binaryWriter, binaryReader, "invalid");
                //museum = new Museum(Client.GetBinaryWriter(), Client.GetBinaryReader(), "//invalid"); //testare trimitere invalida
                Exhibit exhibit = new Exhibit(binaryWriter, binaryReader, "Tablou_de_test");
                /*//exhibit = new Exhibit(binaryWriter, binaryReader, "/fasfa.fsdfs3/';[");
                // exhibit = new Exhibit(Client.GetBinaryWriter(), Client.GetBinaryReader(), "//fasfa.fsdfs3/';[]fsda");
                */
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

        public static bool CheckPacketError(byte[] packet)
        {
            for (int i = 0; i < packet.Length; i++)
            {
                if (packet[i] != 0)
                {
                    return true;
                }
            }
            return false;
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
        internal static String ReceiveZip()
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
                bool ok = false;
                //string filename = ".//Resources//" + packet.type + ".zip";
                Stream fs = null;
                BinaryWriter bw = null;

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

                    if(!ok && !packet.type.Equals("[Error]"))
                    {
                        packet.type = packet.type.Replace("\0", String.Empty);
                        string filename = ".//Resources//" + GetPacketNameFromPacketType(packet.type) + ".zip";
                        fs = new FileStream(filename, FileMode.Append);
                        bw = new BinaryWriter(fs);
                        ok = true;
                    }

                    bw.Write(packet.data);
                    bw.Flush();
                    Console.WriteLine("[" + DateTime.Now + "] Packet received!");
                }
                fs.Close();
                bw.Close();
                return packet.type;
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


        public static void SendText(String packetType, String text)
        {
            Send("[" + packetType + "]",  Encoding.UTF8.GetBytes(text));
            Console.WriteLine("[" + DateTime.Now + "] Text Sent!");
        }

    
        internal static byte[] packetToBytes(Packet packet)
        {
            try
            {

                byte[] packetBytes = Encoding.UTF8.GetBytes(packet.type);
                Array.Resize<byte>(ref packetBytes, Constants.type_length + packet.data.Length);
                Array.Copy(packet.data, 0, packetBytes, Constants.type_length, packet.data.Length);
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

        public static string GetPacketNameFromPacketType(String path)
        {
            Regex regex = new Regex(".+-([\\w\\s]+\\w)", RegexOptions.IgnoreCase);
            Match match = regex.Match(path);
            if (match.Success)
            {
                return match.Groups[1].ToString();
            }
            return null;
        }



    }



}
