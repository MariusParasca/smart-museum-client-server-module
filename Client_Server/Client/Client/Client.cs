using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Drawing;

namespace Client
{

        public class Client
        {
        private static Cryptor cryptor;

        internal static Cryptor Cryptor { get => cryptor; set => cryptor = value; }

        public static void Main()
            {
                Cryptor = new Cryptor();
                try
                {
                    TcpClient tcpclnt = new TcpClient();
                    Console.WriteLine("Connecting.....");
                    tcpclnt.Connect("127.0.0.1", 8001);
                    Console.WriteLine("Connected");
                    Stream stm = tcpclnt.GetStream();



                    //trimitere text
                    SendText(stm, "Text de test");
                    //primire text
                    Console.WriteLine(ReceiveText(stm));

                    ReceivePhoto(stm, "test.jpg");



                Console.WriteLine("\nJob done! Now exit!");
                    tcpclnt.Close();
                }

                catch (Exception e)
                {
                    Console.WriteLine("Error..... " + e.StackTrace);
                }
            }

            public static void SendText(Stream stream, String text)
            {
                Console.WriteLine("[TEXT] Transmitting.....");
                ASCIIEncoding asen = new ASCIIEncoding();
                byte[] toSend = asen.GetBytes(text);              
                stream.Write(toSend, 0, toSend.Length);
                Console.WriteLine("[TEXT] Text was send.");
            }



            public static String ReceiveText(Stream stream)
            {
                int howBig = 100000;
                byte[] bb = new byte[howBig];
                int k = stream.Read(bb, 0, howBig);
                Console.WriteLine("[TEXT] Received \n");
                return bArrayToString(bb,k);
            }

            public static void ReceivePhoto(Stream stream, String fileName)
            {
                int howBig = 10000000;
                byte[] bb = new byte[howBig];
                int k = stream.Read(bb, 0, howBig);
                using (var ms = new MemoryStream(bb))
                {
                    Image.FromStream(ms).Save(fileName);
                    Console.WriteLine("[PHOTO] Received \n");
                }
            }

            public static String bArrayToString(byte[] byteArray, int len)
            {
                return System.Text.Encoding.UTF8.GetString(byteArray, 0, len);             
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

            private static byte[] getContent(byte[] byteArray)
            {
                return byteArray.Skip(4).Take(byteArray.Length-4).ToArray();
            }






    }

    

}
