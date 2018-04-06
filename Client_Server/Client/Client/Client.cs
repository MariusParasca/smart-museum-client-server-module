using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Client
{


        public class Client
        {

            public static void Main()
            {

                try
                {
                    TcpClient tcpclnt = new TcpClient();
                    Console.WriteLine("Connecting.....");

                    tcpclnt.Connect("127.0.0.1", 8001);
                    // use the ipaddress as in the server program

                    Console.WriteLine("Connected");
                 
                    String str = "test";

                    Stream stm = tcpclnt.GetStream();

                    ASCIIEncoding asen = new ASCIIEncoding();
                    byte[] ba = asen.GetBytes("asd");
                    Console.WriteLine("Transmitting.....");

                    stm.Write(ba, 0, ba.Length);

                    byte[] bb = new byte[100];
                    int k = stm.Read(bb, 0, 100);

                    for (int i = 0; i < k; i++)
                        Console.Write(Convert.ToChar(bb[i]));

                    tcpclnt.Close();
                }

                catch (Exception e)
                {
                    Console.WriteLine("Error..... " + e.StackTrace);
                }
            }

        }

        //

}
