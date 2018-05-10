//
/*   Server Program    */

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Drawing;
using System.IO;
using client_server;
using System.Linq;
using MySql.Data.MySqlClient;

namespace Server
{
    public class Server
    {
        private static Cryptor cryptor;
        private static Compresser Compresser;
        //internal static Cryptor Cryptor { get => cryptor; set => cryptor = value; }

        public static void Main()
        {
            Museum.CreateGeoLocaitonFile();

            SFTP sftp = new SFTP();
            //Cryptor = new Cryptor();
            Compresser = new Compresser();
            
            try
            {
                
                IPAddress ipAd = IPAddress.Parse("127.0.0.1");
                
                TcpListener myList = new TcpListener(ipAd, 8001);
                myList.Start();

                Console.WriteLine("The server is running at port 8001...");
                Console.WriteLine("The local End point is  :" + myList.LocalEndpoint);
                Console.WriteLine("Waiting for a connection.....");

                Socket s = myList.AcceptSocket();
                Console.WriteLine("Connection accepted from " + s.RemoteEndPoint);

                /*
                SendInt(s, 10);
                SendString(s, "Ana are mere");
                Console.WriteLine("Numarul primit este: " + ReceiveInt(s));
                Console.WriteLine("Mesajul primit este: " + ReceiveString(s));
                RecieveZip(s);
                */
                
                bool running = true;
                while (running == true)
                {
                    int command = ReceiveInt(s);
                    Console.WriteLine("Comanda: " + command);
                    if (command == 1)
                    {
                        String nume = ReceiveString(s);
                        String prenume = ReceiveString(s);
                        SendString(s, "Conectat!");
                    }

                    if (command == 2)
                    {
                        String nume = ReceiveString(s);
                        String prenume = ReceiveString(s);
                        int varsta = ReceiveInt(s);
                        SendString(s, "Inregistrat!");
                    }

                    if (command == 3)
                        running = false;
                        
                }
                
                /*
                String museumName = ReceiveText(s);
                String museumPath = db.GetPath(museumName);
                sftp.GetMuseumPackage(museumPath);
                //Send(s, sftp.GetMuseumPackage(museumPath));


                Console.WriteLine(ReceiveText(s));
                SendText(s, "Mesaj");
                
                //   SendPhoto(s, "G:\\Doc\\smart-museum-client-server-module\\Client_Server\\meme.jpg");
                //SendPhoto(s, "C:\\Users\\abucevschi\\Desktop\\smart-museum-client-server-module\\Client_Server\\meme.jpg");
                SendPhoto(s, "E:\\Dropbox\\Facultate\\IP\\Proiect\\Client_Server\\meme.jpg");
                */

                s.Close();
                myList.Stop();
                
            }           
            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.StackTrace);
            }
            
        }

        public static void SendInt(Socket socket, int number)
        {
            socket.Send(BitConverter.GetBytes(number));

            Console.WriteLine("Numarul: " + number + "a fost trimis");
        }

        public static void SendString(Socket socket, String text)
        {
            byte[] arr = System.Text.Encoding.ASCII.GetBytes(text);
            SendInt(socket, arr.Length);
            socket.Send(arr);
            Console.WriteLine("Mesajul" + text + " a fost trimis");
        }

        public static int ReceiveInt(Socket socket)
        {
            BinaryReader binaryReader = new BinaryReader(new NetworkStream(socket));
            int number = binaryReader.ReadInt32();
            return number;
        }

        public static string ReceiveString(Socket socket)
        {
            int length = ReceiveInt(socket);
            byte[] b = new byte[length];
            BinaryReader binaryReader = new BinaryReader(new NetworkStream(socket));
            int nrBytes = binaryReader.Read(b, 0, length);
            return bArrayToString(b, nrBytes);
        }

        public static void RecieveZip(Socket socket)
        {
            BinaryReader binaryReader = new BinaryReader(new NetworkStream(socket));
            int length = ReceiveInt(socket);
            byte[] b = new byte[length];
            int nrBytes = binaryReader.Read(b, 0, length);
            Console.WriteLine(length);
            File.WriteAllBytes("D:\\Client_Server\\meme(copy).zip", b);
        }

        public static String ReceiveText(Socket socket)
        {
            BinaryReader binaryReader = new BinaryReader(new NetworkStream(socket));
            int howBig = binaryReader.ReadInt32();
            byte[] bb = new byte[howBig];
            int k = binaryReader.Read(bb, 0, howBig);
            Console.WriteLine("[TEXT] Received \n");
            return bArrayToString(bb, k);
        }
        private static int CalculateChecksum(byte[] byteArray)
        {
            int checksum = 0;
            foreach (byte chData in byteArray)
                checksum += chData;
            return checksum;
        }
  
        public static String bArrayToString(byte[] byteArray, int len)
        {
            string str = System.Text.Encoding.UTF8.GetString(byteArray, 0, len);
            return str;
        }

        public static void SendText(Socket socket, String text)
        {
            ASCIIEncoding asen = new ASCIIEncoding();
            Send(socket, asen.GetBytes(text));
            Console.WriteLine("\n[TEXT] Message Send");
        }
            
        public static void SendPhoto(Socket socket, String imagePath)
        {
            Bitmap bitmap = new Bitmap(imagePath);
            byte[] imageByte = ImageToByteArray(bitmap);
            Send(socket, imageByte);
            Console.WriteLine("\n[IMAGE] Image Send");
        }
        public static void Send(Socket socket, byte[] byteArray)
        {
            byteArray = Compresser.Compress(byteArray);
            socket.Send(BitConverter.GetBytes(byteArray.Length));
            socket.Send(byteArray);
            int checkSum = CalculateChecksum(byteArray);
            socket.Send(BitConverter.GetBytes(checkSum));
        }

        public static byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, imageIn.RawFormat);
                return ms.ToArray();
            }
        }

        public static byte[] addLength(byte[] baseArray, int len)
        {

            byte[] lenBytes = BitConverter.GetBytes(len);
            byte[] rv = new byte[lenBytes.Length + len];
            System.Buffer.BlockCopy(lenBytes, 0, rv, 0, lenBytes.Length);
            System.Buffer.BlockCopy(baseArray, 0, rv, lenBytes.Length, len);
            return rv;
        }

        private static int getSize(byte[] byteArray)
        {
            return BitConverter.ToInt32(byteArray.Take(4).ToArray(), 0);
        }

        private static byte[] getContent(byte[] byteArray, int len)
        {
            return byteArray.Skip(4).Take(len).ToArray();
        }

        private static void printBArray(byte[] byteArray, int len)
        {
            Console.WriteLine(" ");
            for (int i = 0; i < len; i++)
            {
                Console.Write(Convert.ToChar(byteArray[i]));
            }
            Console.WriteLine(" ");
        }

    }
}
