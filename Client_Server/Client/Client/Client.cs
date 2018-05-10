using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;

public struct Packet
{
    public String type;
    public byte[] data;
}
namespace Client
{

        public class Client
        {
        private static Cryptor cryptor;
        private static Exhibit exhibit;
        internal static Cryptor Cryptor { get => cryptor; set => cryptor = value; }
        private static Compresser Compresser;
        public static void Main()
            {
                Cryptor = new Cryptor();
                Compresser = new Compresser();
                /*exhibit = new Exhibit();
                exhibit.LoadJson("..\\..\\..\\exhibit.json");
                exhibit.show();
                Museum museum = new Museum(null, null, "Muzeu de test");
                museum.CreateExhibits("..\\..\\..\\muzeu_de_test");*/
            try
                 {

                     TcpClient tcpclnt = new TcpClient();
                     Console.WriteLine("Connecting.....");
                     tcpclnt.Connect("127.0.0.1", 8001);
                     Console.WriteLine("Connected");
                     BinaryReader binaryReader = new BinaryReader(tcpclnt.GetStream());
                     BinaryWriter binaryWriter = new BinaryWriter(tcpclnt.GetStream());
                     

                //trimitere text
                SendText(binaryWriter, "Text de test");
                    //primire text
                    Console.WriteLine(ReceiveText(binaryReader));
                    


                ReceivePhoto(new BinaryReader(tcpclnt.GetStream()), "test.jpg");
                
               

                Console.WriteLine("byte array file recevied");

                
                Console.WriteLine("\nJob done! Now exit!");
                    tcpclnt.Close();
                }

                catch (Exception e)
                {
                    Console.WriteLine("Error..... " + e.StackTrace);
                }
                
            }

        private static void ReceivePhoto(BinaryReader binaryReader, string v)
        {
        }


        public static String ReceiveText(BinaryReader binaryReader)
        {
            Packet packet = Receive(binaryReader);
            return bArrayToString(packet.data, packet.data.Length);
        }
        private static Packet bytesToPacket(byte[] arr)
        {
            Packet str = new Packet();

            int size = Marshal.SizeOf(str);
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(arr, 0, ptr, size);

            str = (Packet)Marshal.PtrToStructure(ptr, str.GetType());
            Marshal.FreeHGlobal(ptr);

            return str;
        }
        public static Packet Receive(BinaryReader binaryReader)
        {
            Packet packet;
            int howBig = binaryReader.ReadInt32();
            byte[] packetBytes = new byte[howBig];
            int readed = binaryReader.Read(packetBytes, 0, howBig);
            int myCheckSum = CalculateChecksum(packetBytes);
            int checkSum = binaryReader.ReadInt32();
            if (myCheckSum != checkSum)
            {
                packet.type = "[Error]";
                packet.data = Encoding.ASCII.GetBytes("Checksum does not match!");
                Console.WriteLine("[" + DateTime.Now + "] [Error] Checksum does not match!");
                return packet;
            }
            byte[] decompressedByteArray = Compresser.Decompress(packetBytes);
            packet = bytesToPacket(decompressedByteArray);
            Console.WriteLine("[" + DateTime.Now + "] Packet received!");
            return packet;

        }
        private static int CalculateChecksum(byte[] packetBytes)
        {
            int checksum = 0;
            foreach (byte chData in packetBytes)
                checksum += chData;
            return checksum;
        }

        public static String bArrayToString(byte[] byteArray, int len)
        {
            string str = Encoding.UTF8.GetString(byteArray, 0, len);
            return str;
        }

        public static void SendText(BinaryWriter binaryWriter, String text)
        {
            ASCIIEncoding asen = new ASCIIEncoding();
            Packet packet;
            packet.type = "[Text]";
            packet.data = asen.GetBytes(text);
            Send(binaryWriter, packet);
            Console.WriteLine("[" + DateTime.Now + "] Text Sent!");
        }
         
        public static void SendPhoto(BinaryWriter binaryWriter, String imagePath)
        {
            Bitmap bitmap = new Bitmap(imagePath);
            byte[] imageByte = ImageToByteArray(bitmap);
            Packet packet;
            packet.type = "[Image]";
            packet.data = imageByte;
            Send(binaryWriter, packet);
            Console.WriteLine("[" + DateTime.Now + "] Image Sent!");
        }
        public static byte[] packetToBytes(Packet packet)
        {
            int size = Marshal.SizeOf(packet);
            byte[] arr = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(packet, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }
        private static void Send(BinaryWriter binaryWriter, Packet packet)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            packet.data = Compresser.Compress(packet.data);
            int size = Marshal.SizeOf(packet);
            binaryWriter.Write(BitConverter.GetBytes(size));
            byte[] packetBytes = packetToBytes(packet);
            binaryWriter.Write(packetBytes);
            Console.WriteLine("[" + DateTime.Now + "] Packet sent!");
            int checkSum = CalculateChecksum(packetBytes);
            binaryWriter.Write(BitConverter.GetBytes(checkSum));
        }

        private static byte[] ImageToByteArray(Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, imageIn.RawFormat);
                return ms.ToArray();
            }
        }

        private static byte[] addLength(byte[] baseArray, int len)
        {

            byte[] lenBytes = BitConverter.GetBytes(len);
            byte[] rv = new byte[lenBytes.Length + len];
            Buffer.BlockCopy(lenBytes, 0, rv, 0, lenBytes.Length);
            Buffer.BlockCopy(baseArray, 0, rv, lenBytes.Length, len);
            return rv;
        }

    }



}
