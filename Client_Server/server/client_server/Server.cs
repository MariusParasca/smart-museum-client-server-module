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

namespace Server
{
    public class Server
    {
        private static Cryptor cryptor;

        internal static Cryptor Cryptor { get => cryptor; set => cryptor = value; }

        public static void Main()
        {
            Cryptor = new Cryptor();
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

                Console.WriteLine(ReceiveText(s,100));
                SendText(s, "Mesaj");
                SendPhoto(s, "G:\\Doc\\smart-museum-client-server-module\\Client_Server\\meme.jpg");

                
                
                s.Close();
                myList.Stop();

            }           
            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.StackTrace);
            }
        }

        public static String ReceiveText(Socket socket, int howBig)
        {
            byte[] bb = new byte[100000];
            int k = socket.Receive(bb);
      
            Console.WriteLine("[TEXT] Received \n");
            return bArrayToString(bb, k);
        }

        public static String bArrayToString(byte[] byteArray, int len)
        {
            string str = System.Text.Encoding.UTF8.GetString(byteArray, 0, len);
            return str;
        }

        public static void SendText(Socket socket, String text)
        {
            ASCIIEncoding asen = new ASCIIEncoding();
            socket.Send(asen.GetBytes(text));
      

            Console.WriteLine("\n[TEXT] Message Send");
        }
            
        public static void SendPhoto(Socket socket, String imagePath)
        {
            Bitmap bitmap = new Bitmap(imagePath);
            byte[] imageByte = ImageToByteArray(bitmap);
            ASCIIEncoding asen = new ASCIIEncoding();           
            socket.Send(imageByte);          
            Console.WriteLine("\n[IMAGE] Image Send");
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
