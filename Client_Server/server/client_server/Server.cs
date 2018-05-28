//
/*   Server Program    */

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using client_server;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Threading;

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
        public class handleClient
        {
            TcpClient clientSocket;
            string clNo;
            public void startClient(TcpClient inClientSocket, string clineNo)
            {
                clientSocket = inClientSocket;
                clNo = clineNo;
                Thread ctThread = new Thread(doChat);
                ctThread.Start();
            }
            private void doChat()
            {
                int requestCount = 0;
                byte[] bytesFrom = new byte[10025];
                requestCount = 0;

                while (true)
                {
                    try
                    {
                        requestCount = requestCount + 1;
                        NetworkStream networkStream = clientSocket.GetStream();

                        Packet packet = new Packet();
                        BinaryReader binaryReader = new BinaryReader(clientSocket.GetStream());
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
                            packet.data = Encoding.ASCII.GetBytes("[server] Checksum does not match!" + myCheckSum + " " + checkSum);
                            Console.WriteLine("{0}-[{1}] {2}", clNo, DateTime.Now, "Checksum does not match!");
                            continue;
                        }

                        packet = bytesToPacket(packetBytes, clNo);
                        Array.Copy(packetBytes, Constants.type_length, data, cnt, howBig - Constants.type_length);
                        cnt += howBig - Constants.type_length;
                        if (packet.type == "[EndT]")
                            break;

                        Console.WriteLine("[Client-{0}]-[{1}] {2}", clNo, DateTime.Now, "Packet received!");

                        packet.type.Replace("\0", string.Empty);
                        if (packet.type.ToLower().StartsWith("[set-museum]") || packet.type.ToLower().StartsWith("[set-exhibit]"))
                            ReceiveZip(networkStream, len, packet, howBig, clNo);
                        else
                            ReceiveText(networkStream, len, packet, howBig, clNo);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(" >> " + ex.ToString());
                    }
                }
            }
        }
        public static void Main()
        {
            SFTP sftp = new SFTP();
            Compresser = new Compresser();
            running = true;
            endPacket.type = "[EndT]";
            Directory.CreateDirectory(".//Resources");
            Directory.CreateDirectory(".//Logs");
            Package.GetExhibitList("Muzeu de test");
            Package.CreateGeoLocationFile();
            Package.Login("Muzeu de test", "parola");
            try
            {
                IPAddress ipAddress = Dns.Resolve("localhost").AddressList[0];//172.30.0.211
                Console.WriteLine("IP: " + ipAddress);
                int port = 8081;
                int counter = 0;
                TcpListener myList = new TcpListener(ipAddress, port);
                TcpClient clientSocket = default(TcpClient);
                myList.Start();
                while (running)
                {
                    counter++;
                    Log();
                    Console.WriteLine("[{0}] {1} {2}...", DateTime.Now, "The server is running at port ", port);
                    Console.WriteLine("[{0}] {1}", DateTime.Now, "The local End point is  :" + myList.LocalEndpoint);
                    Console.WriteLine("[{0}] {1}", DateTime.Now, " Waiting for a connection.....");

                     clientSocket = myList.AcceptTcpClient();
                    handleClient client = new handleClient();
                    client.startClient(clientSocket, Convert.ToString(counter));
                    var clientPort = ((IPEndPoint)clientSocket.Client.RemoteEndPoint).Port;
                    Console.WriteLine("[{0}]Connection accepted from {1}", DateTime.Now, ((IPEndPoint)clientSocket.Client.RemoteEndPoint));
                }
                myList.Stop();


            }
            catch (Exception e)
            {
                Log();
                Console.WriteLine("Error..... {0} {1}", e.GetType().ToString(), e.StackTrace);
            }

        }
        internal static string ReceiveText(NetworkStream networkStream, int len, Packet packet, int cat, string clNo)
        {
            string type = packet.type.Replace("\0", string.Empty);
            byte[] data = Receive(networkStream, len, packet, cat, clNo);
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
                    SendText(networkStream, Package.GetExhibitList(museum), clNo);
                }
                else
                {
                    SendText(networkStream, "Invalid user or password! Please try again!", clNo);
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
                    Send(networkStream, "[Error]", err, clNo);

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
                    Send(networkStream, "[Error]", err, clNo);

                }
                else
                    SendZip(networkStream, "[Museum]-" + GetPacketNameFromPath(path), path, clNo);


            }
            else
                if (type.Equals("[get-exhibit]"))
            {
                string path = Package.GetPath("SmartMuseumDB.Exhibits", str);
                if (!File.Exists(path))
                {
                    byte[] err = Encoding.ASCII.GetBytes("Exhibit invalid path");
                    Send(networkStream, "[Error]", err, clNo);

                }
                else
                    SendZip(networkStream, "[Exhibit]-" + GetPacketNameFromPath(path), path, clNo);


            }
            else
                if (type.Equals("[get-exhibit-list]"))
            {
                string exhibits = Package.GetExhibitList(str);
                SendText(networkStream, exhibits,clNo);

            }



            return str;
        }

        private static Packet bytesToPacket(byte[] arr, string clNo)
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
                Console.WriteLine("[Client-{0}]-[{1}] {2} {3} {4}", clNo, DateTime.Now, "Error....", e.GetType().ToString(), e.StackTrace);
                Packet packet = new Packet();
                packet.type = "[Error]";
                // am adaugat linia de cod pentru cazul in care muzeul nu este gasit in baza de date
                packet.data = new byte[] { 0 };
                return packet;
            }
        }
        internal static byte[] Receive(NetworkStream networkStream, int len, Packet packet, int cat, string clNo)
        {

            Log();
            try
            {
                int cnt = 0;
                byte[] data = new byte[len + Constants.data_length];
                byte[] packetBytes = new byte[Constants.data_length + Constants.type_length];
                packetBytes = packetToBytes(packet, clNo);
                Array.Copy(packetBytes, Constants.type_length, data, cnt, cat - Constants.type_length);
                cnt += cat;
                BinaryReader binaryReader = new BinaryReader(networkStream);
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
                        Console.WriteLine("[Client-{0}]-[{1}] {2}", clNo, DateTime.Now, "[Error] Checksum does not match!");
                        return null;
                    }
                    packet = bytesToPacket(packetBytes, clNo);
                    Array.Copy(packetBytes, Constants.type_length, data, cnt, howBig - Constants.type_length);
                    if (packet.type == "[EndT]")
                        break;

                  //  Console.WriteLine("[" + DateTime.Now + "] Packet received!");


                }
                return data;
            }
            catch (Exception e)
            {
                Log();
                Console.WriteLine("[Client-{0}]-[{1}] {2}", clNo, DateTime.Now, "Error..... " + e.GetType().ToString() + " " + e.StackTrace);
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

        public static void SendText(NetworkStream networkStream, string text, string clNo)
        {
            Log();
            byte[] data = Encoding.UTF8.GetBytes(text);
            Send(networkStream, "[Text]", data, clNo);
            Console.WriteLine("[Client-{0}]-[{1}] {2}", clNo, DateTime.Now, "Text Sent!");
        }

        private static byte[] packetToBytes(Packet packet, string clNo)
        {
            Log();
            try
            {

                byte[] packetBytes = Encoding.UTF8.GetBytes(packet.type);
                Array.Resize(ref packetBytes, Constants.type_length + packet.data.Length);
                Array.Copy(packet.data, 0, packetBytes, Constants.type_length, packet.data.Length);
                return packetBytes;
            }
            catch (Exception e)
            {
                Log();
                Console.WriteLine("[Client-{0}] [{1}] Error..... {} {}", clNo, DateTime.Now, e.GetType().ToString(), e.StackTrace);
                return null;
            }
        }
        private static void Send(NetworkStream networkStream, string type, byte[] data, string clNo)
        {
            try
            {
                Packet packet = new Packet();
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                BinaryWriter binaryWriter = new BinaryWriter(networkStream);
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
                    packetBytes = packetToBytes(packet, clNo);
                    size = packetBytes.Length;
                    binaryWriter.Write(BitConverter.GetBytes(size), 0, 4);
                    binaryWriter.Write(packetBytes, 0, size);
                    checkSum = CalculateChecksum(packetBytes);
                    binaryWriter.Write(BitConverter.GetBytes(checkSum), 0, 4);
                    binaryWriter.Flush();
                    cnt += x;
                }


            }

            catch (Exception e)
            {
                Log();
                Console.WriteLine("[Client-{0}]-[{1}] {2}", clNo, DateTime.Now, "Error..... " + e.GetType().ToString() + " " + e.StackTrace);
            }
        }

        private static void SendZip(NetworkStream networkStream, string type, string filePath, string clNo)
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
                BinaryWriter binaryWriter = new BinaryWriter(networkStream);

                binaryWriter.Write(BitConverter.GetBytes(len), 0, 4);
                while (cnt < len)
                {
                    x = Math.Min(Constants.data_length, len - cnt);
                    packet.type = type;
                    int read = fs.Read(data, 0, Constants.data_length);
                    Array.Copy(data, 0, packet.data, 0, x);
                    packetBytes = packetToBytes(packet, clNo);
                    size = packetBytes.Length;
                    binaryWriter.Write(BitConverter.GetBytes(size), 0, 4);
                    binaryWriter.Write(packetBytes, 0, size);
                    checkSum = CalculateChecksum(packetBytes);
                    binaryWriter.Write(BitConverter.GetBytes(checkSum), 0, 4);
                    binaryWriter.Flush();
                    cnt += x;
                }


            }

            catch (Exception e)
            {
                Log();
                Console.WriteLine("[Client-{0}]-[{1}] {2}", clNo, DateTime.Now, "Error..... {0} {1} {2}", e.GetType().ToString() ,e.StackTrace , ((SocketException)e).ErrorCode);
            }
        }
        internal static string ReceiveZip(NetworkStream networkStream, int len, Packet packet, int cat, string clNo)
        {

            try
            {
                BinaryReader binaryReader = new BinaryReader(networkStream);
                packet.data = new byte[Constants.data_length];
                int cnt = 0;
                byte[] data = new byte[len + Constants.data_length];
                byte[] packetBytes = new byte[Constants.data_length + Constants.type_length];
                DateTime dateTime = DateTime.Now;
                bool ok = false;
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
                        Console.WriteLine("[Client-{0}] [{1}] [Error] Checksum does not match!", clNo, DateTime.Now);
                        return null;
                    }
                    packet = bytesToPacket(packetBytes, clNo);
                    cnt += howBig - Constants.type_length;

                    if (!ok)
                    {
                        packet.type = packet.type.Replace("\0", string.Empty);
                        string filename = ".//Resources//" + packet.type + ".zip";
                        fs = new FileStream(filename, FileMode.Append);
                        bw = new BinaryWriter(fs);
                        ok = true;
                    }

                    bw.Write(packet.data);
                    bw.Flush();
                    Console.WriteLine("[Client-{0}]-[{1}] {2}", clNo, DateTime.Now, "Packet received!");
                }
                fs.Close();
                bw.Close();
                return packet.type;
            }
            catch (Exception e)
            {
                Log();
                Console.WriteLine("[Client-{0}]-[{1}] {2} {3} {4}", clNo, DateTime.Now, "Error.....", e.GetType().ToString(), e.StackTrace);
                return null;

            }

        }

        public static string GetPacketNameFromPath(string path)
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
