using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace client_server
{
    class SFTP
    {
        SftpClient client;

        public bool OpenConnection()
        {
            try
            {
                client = new SftpClient("ec2-52-37-205-247.us-west-2.compute.amazonaws.com",
                                    22, "marius", "parola25");
                client.Connect();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.StackTrace);
                return false;
            }
        }

        public byte[] GetMuseumPackage(String path)
        {
            if(OpenConnection() == true)
            {
                Console.WriteLine("Museum: " + path);
                using (Stream fileStream = File.Create(
                    @"E:\Dropbox\Facultate\IP\Proiect\Client_Server\muzeu_de_test.zip"))
                {
                    client.DownloadFile(path, fileStream);
                    Console.WriteLine("[Package] received");

                    return null;
                }       
            }
            return null;
        }
    }
}
