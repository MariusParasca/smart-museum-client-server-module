using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public abstract class Package
    {
        protected BinaryWriter outStream;
        protected BinaryReader inStream;
        protected string name = null;

        protected bool GetPackage(String packetType, String pathToPackage, String folderName)
        {
            try
            {
                Client.SendText(packetType, name);
                if(Client.ReceiveZip(folderName) == null)
                {
                    Console.WriteLine("Invalid package name {0}", name);
                    Console.WriteLine("Invalid package name or other error");
                    return false;
                }
                else
                {
                    Compresser.DecompressZip(pathToPackage + ".zip", pathToPackage);
                    
                    File.Delete(@pathToPackage);
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception. Exception: " + e.ToString());
            }
            return false;
        }

    }
}
