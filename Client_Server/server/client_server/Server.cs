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
[StructLayout(LayoutKind.Sequential, Size = 1020)]
//[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct Packet
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
    public byte[] type;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1014)]
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

                Socket s = myList.AcceptSocket();
                Console.WriteLine("[" + DateTime.Now + "]Connection accepted from " + s.RemoteEndPoint);

                /*
                SendInt(s, 10);
                SendString(s, "Ana are mere");
                Console.WriteLine("Numarul primit este: " + ReceiveInt(s));
                Console.WriteLine("Mesajul primit este: " + ReceiveString(s));
                RecieveZip(s);
                */

                /*
                String museumName = ReceiveText(s);
                String museumPath = db.GetPath(museumName);
                sftp.GetMuseumPackage(museumPath);
                //Send(s, sftp.GetMuseumPackage(museumPath));


                Console.WriteLine(ReceiveText(s));
                SendText(s, "Mesaj");
                
                //   SendPhoto(s, "G:\\Doc\\smart-museum-client-server-module\\Client_Server\\meme.jpg");
                SendPhoto(s, "C:\\Users\\abucevschi\\Desktop\\smart-museum-client-server-module\\Client_Server\\meme.jpg");
                SendPhoto(s, "E:\\Dropbox\\Facultate\\IP\\Proiect\\Client_Server\\meme.jpg");
                */
                //SendPhoto(s, "C:\\Users\\abucevschi\\Desktop\\smart-museum-client-server-module\\Client_Server\\meme.jpg");
                string str = ReceiveText(s);
                Console.WriteLine("Text *"+ str+ "*#");
                Console.Write("****");
            //    System.Diagnostics.Debug.Write("HERE");

                /*   SendPhoto(s, "C:\\Users\\abucevschi\\Desktop\\smart-museum-client-server-module\\Client_Server\\meme.jpg");
                   SendText(s, "asd");*/
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
            Console.WriteLine("type:"+bArrayToString(packet.type, packet.type.Length)+"\n");
            return bArrayToString(packet.data, packet.data.Length);
        }
        private static Packet bytesToPacket(byte[] arr)
        {
            try
            {
                int rawsize = Marshal.SizeOf(typeof(Packet));
                //     if (rawsize > arr.Length)
                //                    return default(Packet);

                IntPtr buffer = Marshal.AllocHGlobal(rawsize);
                Marshal.Copy(arr, 0, buffer, arr.Length);
                Packet packet = (Packet)Marshal.PtrToStructure(buffer, typeof(Packet));
                Marshal.FreeHGlobal(buffer);
                return packet;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Packet packet = new Packet();
                packet.type = Encoding.ASCII.GetBytes("[Error]");
                return packet;
            }
        }
        public static Packet Receive(Socket socket)
        {

            Packet packet;
            try
            {

                BinaryReader binaryReader = new BinaryReader(new NetworkStream(socket));
                int howBig = binaryReader.ReadInt32();
                byte[] packetBytes = new byte[howBig];
                Console.WriteLine("size " + howBig);
                int readed = binaryReader.Read(packetBytes, 0, howBig);
                Console.WriteLine("readed " + readed);
                int myCheckSum = CalculateChecksum(packetBytes);
                int checkSum = binaryReader.ReadInt32();
                if (myCheckSum != checkSum)
                {
                    packet.type = Encoding.ASCII.GetBytes("[Error]");
                    packet.data = Encoding.ASCII.GetBytes("Checksum does not match!" + myCheckSum + " " + checkSum);
                    Console.WriteLine("[" + DateTime.Now + "] [Error] Checksum does not match!");
                    return packet;
                }
                //    packet = bytesToPacket(packetBytes);
                //packet.data = packetBytes;
             //   byte[] decompressedByteArray = Compresser.Decompress(packetBytes);
                packet = bytesToPacket(packetBytes);
                Console.WriteLine("[" + DateTime.Now + "] Packet received!");
                return packet;
            }
            catch (Exception e)
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

        public static string bArrayToString(byte[] byteArray, int len)
        {
            string str = Encoding.ASCII.GetString(byteArray, 0, len);
            Console.WriteLine(len);
            Console.WriteLine( str.Length);
            return str;
        }

        public static void SendText(Socket socket, String text)
        {
            //    ASCIIEncoding asen = new ASCIIEncoding();
            Packet packet;
            packet.type = Encoding.ASCII.GetBytes("[Text]");
            packet.data = Encoding.ASCII.GetBytes(text);
            Send(socket, packet);
            Console.WriteLine("[" + DateTime.Now + "] Text Sent");
        }

        public static void SendPhoto(Socket socket, String imagePath)
        {
            Bitmap bitmap = new Bitmap(imagePath);
            byte[] imageByte = ImageToByteArray(bitmap);
            Packet packet;
            packet.type = Encoding.ASCII.GetBytes("[Image]");
            packet.data = imageByte;
            Send(socket, packet);
            Console.WriteLine("[" + DateTime.Now + "] Image Sent");
        }
        private static byte[] packetToBytes(Packet packet)
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
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }
        private static void Send(Socket socket, Packet packet)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            packet.data = Compresser.Compress(packet.data);
            int size = Marshal.SizeOf(packet);
            Console.WriteLine(size);
            socket.Send(BitConverter.GetBytes(size));
            //   byte[] packetBytes = packetToBytes(packet);
            socket.Send(packet.data);
            Console.WriteLine("[" + DateTime.Now + "] Packet sent!");
            int checkSum = CalculateChecksum(packet.data);
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
