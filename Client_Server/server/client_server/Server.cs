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

namespace Server
{
    public class Server
    {
        private static Compresser Compresser;
        private static StreamWriter sw;
        public static void Log()
        {
            DateTime dateTime = DateTime.Today;
            string logFileName = ".\\Logs\\" + dateTime.ToString("dd_MM_yyyy") + ".log";
            if( sw == null)
                sw = File.AppendText(logFileName);
            sw.AutoFlush = true;
            Console.SetOut(sw);

        }
        public static void Main()
        {
            //Museum.CreateGeoLocationFile();
            //Museum.GetExhibitList("Muzeu de test");
            //Museum.GetPackage("SmartMuseumDB.Museums", "Muzeu de test");
            SFTP sftp = new SFTP();
            Compresser = new Compresser();
              
            try
            {

                IPAddress ipAd = IPAddress.Parse("127.0.0.1");

                TcpListener myList = new TcpListener(ipAd, 8001);
                myList.Start();
                Log();
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
                   */
                Console.WriteLine(ReceiveText(s));
                SendPhoto(s, ".//Resources//meme.jpg");
                string str = ReceiveText(s);
                SendText(s, "asd");
                s.Close();
                myList.Stop();

            }
            catch (Exception e)
            {
                Log();
                Console.WriteLine("Error..... " + e.StackTrace);
            }

        }
        public static string ReceiveText(Socket socket)
        {
            Packet packet = Receive(socket);
            if(packet.type != "[Text]")
            {
                Console.WriteLine("[" + DateTime.Now + "] Type is not [Text]");
                return "[Error]";
            }
            return bArrayToString(packet.data, packet.data.Length);
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
                Log();
                Console.WriteLine(e.ToString());
                Packet packet = new Packet();
                packet.type = "[Error]";
                return packet;
            }
        }
        internal static Packet Receive(Socket socket)
        {

            Log();
            try
            {
                Packet packet;
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
                    packet.type = "[Error]";// Encoding.ASCII.GetBytes("[Error]");
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
            return str;
        }

        public static void SendText(Socket socket, String text)
        {
            Log();
            Packet packet;
            packet.type = "[Text]";
            Console.WriteLine(text);
            packet.data = Encoding.UTF8.GetBytes(text);
            Send(socket, packet);
            Console.WriteLine("[" + DateTime.Now + "] Text Sent!");
        }

        public static void SendPhoto(Socket socket, String imagePath)
        {
            Bitmap bitmap = new Bitmap(imagePath);
            byte[] imageByte = ImageToByteArray(bitmap);
            Packet packet;
            packet.type = "[Image]";//Encoding.ASCII.GetBytes("[Image]");
            packet.data = imageByte;
            Send(socket, packet);
            Console.WriteLine("[" + DateTime.Now + "] Image Sent");
        }
        private static byte[] packetToBytes(Packet packet)
        {
            Log();
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
        private static void Send(Socket socket, Packet packet)
        {
            Log();
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            byte[] packetBytes = packetToBytes(packet);
            int size = packetBytes.Length;
            socket.Send(BitConverter.GetBytes(size), 4, SocketFlags.None);
            socket.Send(packetBytes, size, SocketFlags.None);
            Console.WriteLine("[" + DateTime.Now + "] Packet sent! ");
            int checkSum = CalculateChecksum(packetBytes);
            Console.WriteLine(checkSum);
            socket.Send(BitConverter.GetBytes(checkSum),  4, SocketFlags.None);
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
