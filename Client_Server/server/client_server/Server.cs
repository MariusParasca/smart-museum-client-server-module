//
/*   Server Program    */

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Drawing;
using System.IO;
using client_server;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;

public struct Packet
{
    public String type;
    public byte[] data;
}
namespace Server
{
    public class Server
    {
        private static Compresser Compresser;
        
        public static void Main()
        {
            //Museum.CreateGeoLocationFile();
            //Museum.GetExhibitList("Muzeu de test");
            //Museum.GetPackage("SmartMuseumDB.Museums", "Muzeu de test");
            StreamWriter sw = File.AppendText("log.txt");
            sw.AutoFlush = true;

            SFTP sftp = new SFTP();
            Compresser = new Compresser();
            Console.SetOut(sw);

            try
            {
                
                IPAddress ipAd = IPAddress.Parse("127.0.0.1");
                
                TcpListener myList = new TcpListener(ipAd, 8001);
                myList.Start();

                Console.WriteLine("[" + DateTime.Now + "] The server is running at port 8001...");
                Console.WriteLine("[" + DateTime.Now + "] The local End point is  :" + myList.LocalEndpoint);
                Console.WriteLine("[" + DateTime.Now + "] Waiting for a connection.....");


                /*//sterge inainte de push
                Socket s;
                while (true)
                {
                    s = myList.AcceptSocket();
                    Console.WriteLine("[" + DateTime.Now + "]Connection accepted from " + s.RemoteEndPoint);
                }
                */

                Socket s = myList.AcceptSocket();
                Console.WriteLine("[" + DateTime.Now + "]Connection accepted from " + s.RemoteEndPoint);

                /*SendInt(s, 10);
                SendString(s, "Ana are mere");
                Console.WriteLine("Numarul primit este: " + ReceiveInt(s));
                Console.WriteLine("Mesajul primit este: " + ReceiveString(s));
                RecieveZip(s);
                */

                
                Console.WriteLine(ReceiveText(s));
                SendText(s, "Mesaj");
                
                //   SendPhoto(s, "G:\\Doc\\smart-museum-client-server-module\\Client_Server\\meme.jpg");
                SendPhoto(s, "C:\\Users\\abucevschi\\Desktop\\smart-museum-client-server-module\\Client_Server\\meme.jpg");
                


                /*
                ReceiveText(s);
                //SendPhoto(s, "E:\\Dropbox\\Facultate\\IP\\Proiect\\Client_Server\\meme.jpg");
                SendPhoto(s, "C:\\Users\\abucevschi\\Desktop\\smart-museum-client-server-module\\Client_Server\\meme.jpg");
                SendText(s, "asd");
                */
                s.Close();
                myList.Stop();
                
            }           
            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.StackTrace);
            }
            
        }
        public static string ReceiveText(Socket socket)
        {
            Packet packet = Receive(socket);
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
        public static Packet Receive(Socket socket)
        {

            Packet packet;
            BinaryReader binaryReader = new BinaryReader(new NetworkStream(socket));
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
            try
            {
                packet = bytesToPacket(packetBytes);
                byte[] decompressedByteArray = Compresser.Decompress(packet.data);
                packet = bytesToPacket(decompressedByteArray);
                Console.WriteLine("[" + DateTime.Now + "] Packet received!");
                return packet;
            }catch(Exception e)
            {
                Console.WriteLine(e.ToString());
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

        public static void SendText(Socket socket, String text)
        {
            ASCIIEncoding asen = new ASCIIEncoding();
            Packet packet;
            packet.type = "[Text]";
            packet.data = asen.GetBytes(text);
            Send(socket, packet);
            Console.WriteLine("[" + DateTime.Now + "] Text Sent");
        }

        public static void SendPhoto(Socket socket, String imagePath)
        {
            Bitmap bitmap = new Bitmap(imagePath);
            byte[] imageByte = ImageToByteArray(bitmap);
            Packet packet;
            packet.type = "[Image]";
            packet.data = imageByte;
            Send(socket, packet);
            Console.WriteLine("[" + DateTime.Now + "] Image Sent");
        }
        private static byte[] packetToBytes(Packet packet)
        {
            int size = Marshal.SizeOf(packet);
            byte[] arr = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(packet, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }
        private static void Send(Socket socket, Packet packet)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            packet.data = Compresser.Compress(packet.data);
            int size = Marshal.SizeOf(packet);
            Console.WriteLine(size);
            socket.Send(BitConverter.GetBytes(size));
            byte[] packetBytes = packetToBytes(packet);
            socket.Send(packetBytes);
            Console.WriteLine("[" + DateTime.Now + "] Packet sent!");
            int checkSum = CalculateChecksum(packetBytes);
            socket.Send(BitConverter.GetBytes(checkSum));
            int x = socket.EndSend(null);
        }

        private static byte[] ImageToByteArray(System.Drawing.Image imageIn)
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
