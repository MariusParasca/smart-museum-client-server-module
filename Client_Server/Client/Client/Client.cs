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
        private static Exhibit exhibit;
        internal static Cryptor Cryptor { get => cryptor; set => cryptor = value; }
        private static Compresser Compresser;
        public static void Main()
            {
                Cryptor = new Cryptor();
                Compresser = new Compresser();
                
                try
                {
                    
                    TcpClient tcpclnt = new TcpClient();
                    Console.WriteLine("Connecting.....");
                    tcpclnt.Connect("127.0.0.1", 8001);
                    Console.WriteLine("Connected");
                    BinaryReader binaryReader = new BinaryReader(tcpclnt.GetStream());
                    BinaryWriter binaryWriter = new BinaryWriter(tcpclnt.GetStream());

                    //trimitere nume muzeu pentru interogare
                    Museum.GetMuseum(binaryWriter, binaryReader, "Muzeu de test zip");
                    //trimitere text
                    SendText(binaryWriter, "Text de test");
                    //primire text
                    Console.WriteLine(ReceiveText(binaryReader));
                    


                ReceivePhoto(new BinaryReader(tcpclnt.GetStream()), "test.jpg");


                
                Console.WriteLine("\nJob done! Now exit!");
                    tcpclnt.Close();

                exhibit = new Exhibit();
                exhibit.LoadJson();
                exhibit.show();

                }

                catch (Exception e)
                {
                    Console.WriteLine("Error..... " + e.StackTrace);
                }
                
            }

            public static void SendText(BinaryWriter binaryWriter, String text)
            {
                Console.WriteLine("[TEXT] Transmitting.....");
                ASCIIEncoding asen = new ASCIIEncoding();
                byte[] toSend = asen.GetBytes(text);
                int size = toSend.Length;
                binaryWriter.Write(BitConverter.GetBytes(size));
                binaryWriter.Write(toSend, 0, size);
                Console.WriteLine("[TEXT] Text was send.");
            }



            public static String ReceiveText(BinaryReader binaryReader)
            {
                byte[] byteArray = Receive(binaryReader);
                Console.WriteLine("[TEXT] Received \n");
                return bArrayToString(byteArray,byteArray.Length);
            }

            public static void ReceivePhoto(BinaryReader binaryReader, String fileName)
            {
                byte[] byteArray = Receive(binaryReader);
                using (var ms = new MemoryStream(byteArray))
                {
                    Image.FromStream(ms).Save(fileName);
                    Console.WriteLine("[PHOTO] Received \n");
                }
            }
        private static int CalculateChecksum(byte[] byteArray)
        {
            int checksum = 0;
            foreach (byte chData in byteArray)
                checksum += chData;
            return checksum;
        }
        public static byte[] Receive(BinaryReader binaryReader)
        {
            int howBig = binaryReader.ReadInt32();
            byte[] byteArray = new byte[howBig];
            int readed = binaryReader.Read(byteArray, 0, howBig);
            int myCheckSum = CalculateChecksum(byteArray);
            int checkSum = binaryReader.ReadInt32();
            if (myCheckSum != checkSum)
                return null;
            byte[] decompressedByteArray = Compresser.Decompress(byteArray);
            return decompressedByteArray;
            
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
