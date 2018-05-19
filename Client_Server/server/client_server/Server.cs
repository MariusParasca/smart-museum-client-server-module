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
        private static bool running;
        private static Packet endPacket;
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
            running = true;
            endPacket.type = "[EndT]";
            try
            {
                IPAddress ipAd = IPAddress.Parse("127.0.0.1");
                TcpListener myList = new TcpListener(ipAd, 8001);
                myList.Start();
                while (running)
                {
                    
                   
                    Log();
                    Console.WriteLine("[" + DateTime.Now + "] The server is running at port 8001...");
                    Console.WriteLine("[" + DateTime.Now + "] The local End point is  :" + myList.LocalEndpoint);
                    Console.WriteLine("[" + DateTime.Now + "] Waiting for a connection.....");

                    Socket socket = myList.AcceptSocket();

                    Console.WriteLine("[" + DateTime.Now + "]Connection accepted from " + socket.RemoteEndPoint);

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
                    // while (socket.Connected)
                    //{
                    //   
                    /*  string str = ReceiveText(socket);

                      SendPhoto(socket, ".//Resources//meme.jpg");
                      SendText(socket, "asd");
                      ReceivePhoto(socket, "final_Test.jpg");*/
                    SendZip(socket, "[Muzeu]", ".//Resources//muzeu_de_test.zip");
                  //  socket.Close();

                    //}

                }
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
            byte[] data = Receive(socket);
            string str = bArrayToString(data, data.Length);
            Console.WriteLine("[" + DateTime.Now + "] Packet received!  " + str);
            return str;
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
        public static void ReceivePhoto(Socket socket, String fileName)
        {
            byte[] data = Receive(socket);
            using (var ms = new MemoryStream(data))
            {
                Image.FromStream(ms).Save(".\\Resources\\" + fileName);
                Console.WriteLine("[PHOTO] Received \n");
            }
        }
        internal static byte[] Receive(Socket socket)
        {

            Log();
            try
            {
                Packet packet = new Packet();
                BinaryReader binaryReader = new BinaryReader(new NetworkStream(socket));
                int len = binaryReader.ReadInt32();
                packet.data = new byte[Constants.data_length];
                int cnt = 0;
                byte[] data = new byte[len + Constants.data_length];
                byte[] packetBytes = new byte[Constants.data_length + Constants.type_length];
                while (cnt < len)
                {
                    int howBig = binaryReader.ReadInt32();
                    int read = binaryReader.Read(packetBytes, 0, howBig);
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
            byte[] data = Encoding.UTF8.GetBytes(text);
            Send(socket,"[Text]", data);
            Console.WriteLine("[" + DateTime.Now + "] Text Sent!");
        }

        public static void SendPhoto(Socket socket, String imagePath)
        {
            Bitmap bitmap = new Bitmap(imagePath);
            byte[] imageBytes = ImageToByteArray(bitmap);
            Send(socket, "[Image]", imageBytes);
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
        private static void Send(Socket socket, string type, byte[] data)
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
                socket.Send(BitConverter.GetBytes(len), 4, SocketFlags.None);
                while (cnt < len)
                {
                    x = Math.Min(Constants.data_length, len - cnt);
                    packet.type = type;
                    Array.Copy(data, cnt, packet.data, 0, x);
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
                int len =(int)new FileInfo(filePath).Length;
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
        internal static byte[] ReceiveZip(Socket socket)
        {

            try
            {
                Packet packet = new Packet();
                BinaryReader binaryReader = new BinaryReader(new NetworkStream(socket));
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
