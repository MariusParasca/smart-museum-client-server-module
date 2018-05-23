using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class Museum
    {
        List<Exhibit> exhibits;
        String name;
        BinaryWriter outStream;
        BinaryReader inStream;

        public List<Exhibit> GetExhibits() { return exhibits; }

        public Museum(BinaryWriter outStream, BinaryReader inStream, String name)
        {
            if (name == null || outStream == null || inStream == null)
            {
                Console.WriteLine("name or outStream or inStream is null");
                return;
            }
            this.name = name;
            this.outStream = outStream;
            this.inStream = inStream;
            exhibits = new List<Exhibit>();
            String path = ".\\Resources\\" + name;
            GetMuseum(path);
            CreateExhibits(path);
        }

        public Museum(String pathToMuseum)
        {
            if (pathToMuseum == null)
            {
                Console.WriteLine("pathToMuseum is null");
                return;
            }
            exhibits = new List<Exhibit>();
            this.name = pathToMuseum.Split('\\').Last();
            CreateExhibits(pathToMuseum); // trebuie dat path-ul muzeului
        }

        private void GetMuseum(String pathToMuseum) // trebuie modificata
        {
            try
            {
                Client.SendText("get-museum", this.name);
                String museumPackage = Client.ReceiveZip();
                Compresser.DecompressZip(pathToMuseum + ".zip", pathToMuseum);
                /*
                bool ok = Client.CheckPacketError(museumPackage);
                if (!ok)
                {
                    Console.WriteLine("Invalid museum name");
                }
                else
                {
                    //trebuie modificat path-ul aferent
                    using (FileStream fs = File.Create(pathToMuseum + ".zip")) //probabil fara extensia .zip
                    {
                        fs.Write(museumPackage, 0, museumPackage.Length);
                    }

                    //trebuie despachetat  zip-ul si apoi apelata createExhibits(path-ul folderului)
                    CreateExhibits(pathToMuseum); // folder hardcodat
                    Console.WriteLine("Package(museum) received");
                }      */          
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception. Exception: " + e.ToString());
            }

        }

        private void CreateExhibits(String museumFolder)
        {
            string[] directoryEntries;
            try
            {
                directoryEntries = Directory.GetDirectories(museumFolder);
                foreach (string directoryName in directoryEntries)
                {
                    Console.WriteLine(directoryName);
                    Exhibit exhibit = new Exhibit(directoryName);
                    exhibits.Add(exhibit);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception. Exception: " + e.ToString());
            }
        }
    }

}