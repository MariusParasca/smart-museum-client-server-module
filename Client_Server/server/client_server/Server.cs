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
            if (sw == null)
                sw = File.AppendText(logFileName);
            sw.AutoFlush = true;
            Console.SetOut(sw);

        }
        public static void Main()
        {
            SFTP sftp = new SFTP();
            Compresser = new Compresser();
            running = true;
            endPacket.type = "[EndT]";
            Package.GetExhibitList("Muzeu de test");
            Package.CreateGeoLocationFile();
            Package.Login("Muzeu de test", "parola");
            Package.GetMuseumId("Muzeu de tesst");
            /*Package.InsertExhibits("Muzeu de test", "test",
                new String[] { "test1", "test2" }, new String[] { "path1", "path2" });
            Package.InsertMuseum("test", 2.342, 3.423, 2.34, "path");
            Package.register("Muzeu de test2", "parola");*/
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

<<<<<<< HEAD
                      SendPhoto(socket, ".//Resources//meme.jpg");
                      SendText(socket, "asd");
                      ReceivePhoto(socket, "final_Test.jpg");*/
                    /*               SendZip(socket, "muzeu_de_test", ".//Resources//muzeu_de_test1.zip");
                                 //  socket.Close();

                                   //SendPhoto(socket, ".//Resources//meme.jpg");
                                   SendText(socket, "asd");
                                   //ReceivePhoto(socket, "final_Test.jpg");
                                   //  socket.Close();

                                   //}
                                   for(int i = 0; i < 1; i++)
                                   {
                                       String museumName = ReceiveText(socket); //primirea numelui muzeului
                                       String path = Museum.GetPath("SmartMuseumDB.Museums", museumName);
                                       //byte[] package = Museum.GetPackage("SmartMuseumDB.Museums", museumName);
                                       //Packet packet = bytesToPacket(package);
                                       SendZip(socket, museumName, path);
                                   }

                                   for (int i = 0; i < 1; i++)
                                   {
                                       String exhibitName = ReceiveText(socket); //primirea numelui exponatului
                                       String path = Museum.GetPath("SmartMuseumDB.Exhibits", exhibitName);
                                       //byte[] package = Museum.GetPackage("SmartMuseumDB.Exhibits", exhibitName);
                                       //Packet packet = bytesToPacket(package);
                                       SendZip(socket, exhibitName, path);
                                   }/**/

                    Packet packet = new Packet();
                    BinaryReader binaryReader = new BinaryReader(new NetworkStream(socket));
                    int len = binaryReader.ReadInt32();
                    packet.data = new byte[Constants.data_length];
                    int cnt = 0;
                    byte[] data = new byte[len + Constants.data_length];
                    byte[] packetBytes = new byte[Constants.data_length + Constants.type_length];
                    int howBig = binaryReader.ReadInt32();
                    int read = binaryReader.Read(packetBytes, 0, howBig);
                    int myCheckSum = CalculateChecksum(packetBytes);
                    int checkSum = binaryReader.ReadInt32();
                    if (myCheckSum != checkSum)
                    {
                        packet.type = "[Error]";
                        packet.data = Encoding.ASCII.GetBytes("Checksum does not match!" + myCheckSum + " " + checkSum);
                        Console.WriteLine("[" + DateTime.Now + "] [Error] Checksum does not match!");

                    }
                    else
                    {
                        packet = bytesToPacket(packetBytes);
                        Array.Copy(packetBytes, Constants.type_length, data, cnt, howBig - Constants.type_length);
                        cnt += howBig - Constants.type_length;
                        if (packet.type == "[EndT]")
                            break;

                        Console.WriteLine("[" + DateTime.Now + "] Packet received!");
                    }
                    packet.type.Replace("\0", string.Empty);
                    if (packet.type.ToLower().StartsWith("[set-museum]") || packet.type.ToLower().StartsWith("[set-exhibit]"))
                        ReceiveZip(socket, len, packet, howBig);
                    else
                        ReceiveText(socket, len, packet, howBig);

                }
                myList.Stop();


            }
            catch (Exception e)
            {
                Log();
                Console.WriteLine("Error..... " + e.StackTrace);
            }

        }
        internal static string ReceiveText(Socket socket, int len, Packet packet, int cat)
        {
            string type = packet.type.Replace("\0", string.Empty);
            byte[] data = Receive(socket, len, packet, cat);
            string str = bArrayToString(data, data.Length);
            str = str.Replace("\0", string.Empty);
            if (type.Equals("[login]"))
            {
                string sep = "!@/";
                string user = str.Split(sep.ToCharArray(), StringSplitOptions.None)[0];
                string password = str.Split(sep.ToCharArray(), StringSplitOptions.None)[1];
                if (Package.Login(user, password))
                {
                    string museum = ""; //trebuie luat din db muzeul pentru userul asta
                    SendText(socket, Package.GetExhibitList(museum));
                }
                else
                {
                    SendText(socket, "Invalid user or password! Please try again!");
                }

            }
            else
                if (type.Equals("[register]"))
            {
                string sep = "!@/";
                string user = str.Split(sep.ToCharArray(), StringSplitOptions.None)[0];
                string password = str.Split(sep.ToCharArray(), StringSplitOptions.None)[1];
                Package.Register(user, password); // fara a trimite un raspuns daca s-a inserat cu success

            }
            else

                if (type.Equals("[delete-museum]"))
            {
                string path = Package.GetPath("Museum", str);
                if (!File.Exists(path))
                {
                    byte[] err = Encoding.ASCII.GetBytes("Museum invalid path");
                    Send(socket, "[Error]", err);

                }
                else
                    File.Delete(@path);
            }
            else

                if (type.Equals("[get-museum]"))
            {

                string path = Package.GetPath("SmartMuseumDB.Museums", str);
                if (!File.Exists(path))
                {
                    byte[] err = Encoding.ASCII.GetBytes("Museum invalid path");
                    Send(socket, "[Error]", err);

                }
                else
                    SendZip(socket, "[Museum]-" + GetPacketNameFromPath(path), path);


            }
            else
                if (type.Equals("[get-exhibit]"))
            {
                string path = Package.GetPath("SmartMuseumDB.Exhibits", str);
                if (!File.Exists(path))
                {
                    byte[] err = Encoding.ASCII.GetBytes("Exhibit invalid path");
                    Send(socket, "[Error]", err);

                }
                else
                    SendZip(socket, "[Exhibit]-" + GetPacketNameFromPath(path), path);


            }
            else
                if (type.Equals("[get-exhibit-list]"))
            {
                string exhibits = Package.GetExhibitList(str);
                SendText(socket, exhibits);

            }



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
                // am adaugat linia de cod pentru cazul in care muzeul nu este gasit in baza de date
                packet.data = new byte[] { 0 };
                return packet;
            }
        }
        internal static byte[] Receive(Socket socket, int len, Packet packet, int cat)
        {

            Log();
            try
            {
                int cnt = 0;
                byte[] data = new byte[len + Constants.data_length];
                byte[] packetBytes = new byte[Constants.data_length + Constants.type_length];
                packetBytes = packetToBytes(packet);
                Array.Copy(packetBytes, Constants.type_length, data, cnt, cat - Constants.type_length);
                cnt += cat;
                BinaryReader binaryReader = new BinaryReader(new NetworkStream(socket));
                packet.data = new byte[Constants.data_length];

                while (cnt < len)
                {
                    int howBig = binaryReader.ReadInt32();
                    int read = binaryReader.Read(packetBytes, 0, howBig);
                    cnt += howBig - Constants.type_length;

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
            Send(socket, "[Text]", data);
            Console.WriteLine("[" + DateTime.Now + "] Text Sent!");
        }

        private static byte[] packetToBytes(Packet packet)
        {
            Log();
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
                    Console.WriteLine("[" + type + "] Packet sent! ");
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
        internal static String ReceiveZip(Socket socket, int len, Packet packet, int cat)
        {

            try
            {
                BinaryReader binaryReader = new BinaryReader(new NetworkStream(socket));
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
                Array.Copy(packetBytes, Constants.type_length, data, cnt, cat - Constants.type_length);
                cnt += cat;

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

                    if (!ok)
                    {
                        packet.type = packet.type.Replace("\0", String.Empty);
                        string filename = ".//Resources//" + packet.type + ".zip";
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

        public static string GetPacketNameFromPath(String path)
        {
            Regex regex = new Regex("\\\\([\\w\\s]+)\\.\\w+", RegexOptions.IgnoreCase);
            Match match = regex.Match(path);
            if (match.Success)
            {
                return match.Groups[1].ToString();
            }
            return null;
        }

    }
}
