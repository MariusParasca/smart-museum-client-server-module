using System;
using System.Text;
using System.IO;
using System.Net.Sockets;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;

public struct Packet
{
    public string type;
    public byte[] data;
}
namespace Client
{

    public class Client
        {
        private static Compresser Compresser;
        private static BinaryReader binaryReader;
        private static BinaryWriter binaryWriter;
        private static TcpClient tcpclnt;

        //Astea sunt create doar pentru test, vor fii sterse probabil
        public static BinaryReader GetBinaryReader() { return binaryReader; }
        public static BinaryWriter GetBinaryWriter() { return binaryWriter; }

        public static void Main()
            {
                Compresser = new Compresser();
            try
            {
                /*
                 TcpClient tcpclnt = new TcpClient();

                 Console.WriteLine("Connecting.....");
                 tcpclnt.Connect("127.0.0.1", 8001);
                 Console.WriteLine("Connected");
                 binaryWriter = new BinaryWriter(tcpclnt.GetStream());
                 binaryReader = new BinaryReader(tcpclnt.GetStream());
                 */
                connectToServer("127.0.0.1", 8001);

                //Creare exhibit invalid

                
                //trimitere text
                SendText( "Text de test");
            ReceivePhoto("test.jpg");


                //primire text
                Console.WriteLine(ReceiveText());
         
            Console.WriteLine("byte array file recevied");
            

            Console.WriteLine("\nJob done! Now exit!");
                tcpclnt.Close();
            }

            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.StackTrace);
            }

        }

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
         public static void ReceivePhoto( String fileName)
        {
            Packet packet = Receive();
            using (var ms = new MemoryStream(packet.data))
            {
                Image.FromStream(ms).Save(fileName);
                Console.WriteLine("[PHOTO] Received \n");
            }
        }
        public static Packet Receive()
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
            catch(Exception e)
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

        public static String bArrayToString(byte[] byteArray, int len)
        {
            string str = Encoding.UTF8.GetString(byteArray, 0, len);
            return str;
        }

        public static void SendText( String text)
        {
            ASCIIEncoding asen = new ASCIIEncoding();
            Packet packet;
            packet.type = "[Text]";
            packet.data = asen.GetBytes(text);
            Send( packet);
            Console.WriteLine("[" + DateTime.Now + "] Text Sent!");
        }
         
        public static void SendPhoto( String imagePath)
        {
            Bitmap bitmap = new Bitmap(imagePath);
            byte[] imageByte = ImageToByteArray(bitmap);
            Packet packet;
            packet.type = "[Image]";
            packet.data = imageByte;
            Send( packet);
            Console.WriteLine("[" + DateTime.Now + "] Image Sent!");
        }
        public static byte[] packetToBytes(Packet packet)
        {
            try
            {


                int size = Marshal.SizeOf(packet);
                byte[] arr = new byte[size];
                IntPtr ptr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(packet, ptr, true);
                Marshal.Copy(ptr, arr, 0, size);
                Marshal.FreeHGlobal(ptr);
                return arr;
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
                binaryWriter.Flush();
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
            catch(Exception e)
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
