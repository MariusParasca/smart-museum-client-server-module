using System;
using System.Text;
using System.IO;
using System.Net.Sockets;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;

//[StructLayout(LayoutKind.Sequential, Size = 1024)]
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct Packet
{
    //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
    public byte[] type;
   // [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1014)]
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

        public static void Main()
        {
            Compresser = new Compresser();
            exhibit = new Exhibit();
            try
            {

                TcpClient tcpclnt = new TcpClient();

                Console.WriteLine("Connecting.....");
                tcpclnt.Connect("127.0.0.1", 8001);
                Console.WriteLine("Connected");
                binaryWriter = new BinaryWriter(tcpclnt.GetStream());
                binaryReader = new BinaryReader(tcpclnt.GetStream());


                //trimitere text
                SendText("Text de test");
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
                packet.type = Encoding.ASCII.GetBytes("[Error]");
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
                    packet.type = Encoding.ASCII.GetBytes("[Error]");
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
            packet.type = Encoding.ASCII.GetBytes("[Text]");
            Console.WriteLine(text);
            //     Console.WriteLine(Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(text)));
            packet.data = Encoding.ASCII.GetBytes(text);
            //   Console.WriteLine(Encoding.ASCII.GetString(packet.data));

            Send(packet);
            Console.WriteLine("[" + DateTime.Now + "] Text Sent!");
        }

        public static void SendPhoto(String imagePath)
        {
            Bitmap bitmap = new Bitmap(imagePath);
            byte[] imageByte = ImageToByteArray(bitmap);
            Packet packet;
            packet.type = Encoding.ASCII.GetBytes("[Image]");
            packet.data = imageByte;
            Send(packet);
            Console.WriteLine("[" + DateTime.Now + "] Image Sent!");
        }
        public static byte[] packetToBytes(Packet packet)
        {
            try
            {

                int rawSize = Marshal.SizeOf(typeof(Packet));
                IntPtr buffer = Marshal.AllocHGlobal(rawSize);
                Marshal.StructureToPtr(packet, buffer, false);
                byte[] rawData = new byte[rawSize];
                Marshal.Copy(buffer, rawData, 0, rawSize);
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
                byte[] packetBytes = packetToBytes(packet);
                packetBytes = Compresser.Compress(packetBytes);
                int size = Marshal.SizeOf(packet);
                byte[] intBytes = BitConverter.GetBytes(packetBytes.Length);
                if (!BitConverter.IsLittleEndian)
                    Array.Reverse(intBytes);
                binaryWriter.Write(intBytes);
                binaryWriter.Write(packetBytes);
                Console.WriteLine("[" + DateTime.Now + "] Packet sent! ");
                int checkSum = CalculateChecksum(packetBytes);
                Console.WriteLine(checkSum);
                intBytes = BitConverter.GetBytes(checkSum);
                if (!BitConverter.IsLittleEndian)
                    Array.Reverse(intBytes);
                binaryWriter.Write(intBytes);
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
