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

                SendText("String de test");
                ReceivePhoto(".\\Resources\\test.jpg");
                Console.WriteLine(ReceiveText());
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
                Console.WriteLine("size " + howBig);
                int readed = binaryReader.Read(packetBytes, 0, howBig);
                Console.WriteLine("readed " + readed);
                int myCheckSum = CalculateChecksum(packetBytes);
                int checkSum = binaryReader.ReadInt32();
                if (myCheckSum != checkSum)
                {
                    packet.type = "[Error]";
                    packet.data = Encoding.ASCII.GetBytes("Checksum does not match!" + myCheckSum + " " + checkSum);
                    Console.WriteLine("[" + DateTime.Now + "] [Error] Checksum does not match!");
                    return packet;
                }
                packet = bytesToPacket(packetBytes);
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
            Packet packet;
            packet.type = "[Text]";
            Console.WriteLine(text);
            packet.data = Encoding.UTF8.GetBytes(text);
            Send(packet);
            Console.WriteLine("[" + DateTime.Now + "] Text Sent!");
        }

        public static void SendPhoto(String imagePath)
        {
            Bitmap bitmap = new Bitmap(imagePath);
            byte[] imageByte = ImageToByteArray(bitmap);
            Packet packet;
            packet.type = "[Image]";
            packet.data = imageByte;
            Send(packet);
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
        private static void Send(Packet packet)
        {
            try
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                byte[] packetBytes = packetToBytes(packet);
                int size = packetBytes.Length;
                binaryWriter.Write(BitConverter.GetBytes(size), 0, 4);
                binaryWriter.Write(packetBytes, 0, size);
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
