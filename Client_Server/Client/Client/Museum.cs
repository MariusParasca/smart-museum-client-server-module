using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Museum
    {
        List<Exhibit> exhibits;
        String name;
        BinaryWriter outStream;
        BinaryReader inStream;

        public Museum (BinaryWriter outStream, BinaryReader inStream, String name)
        {
            if(name == null || outStream == null || inStream == null)
            {
                Console.WriteLine("name or outStream or inStream is null");
                return;
            }
            this.name = name;
            this.outStream = outStream;
            this.inStream = inStream;
            exhibits = new List<Exhibit>();
            //GetMuseum(name);
        }

        public Museum(String pathToMuseum) 
        {
            if(pathToMuseum == null)
            {
                Console.WriteLine("pathToMuseum is null");
                return;
            }
            CreateExhibits(pathToMuseum); // trebuie dat path-ul muzeului
        }

        public void GetMuseum(String name) // modifica in privat
        {
            try
            {
                Client.SendText(outStream, name);
                Packet museumPackage = Client.Receive(inStream);
                using (FileStream fs = File.Create("..\\..\\..\\Tablou_de_test.zip")) //de inlocuit cu path-ul cu folderul muzee + name(adica numele muzeului);
                {
                    //fs.Write(museumPackage, 0, museumPackage.data.Length);
                }
                //trebuie despachetat  zip-ul si apoi apelata createExhibits(path-ul folderului)
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception. Exception: " + e.ToString());
            }

        }

        public void CreateExhibits(String museumFolder) // modifica in privat
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
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine("Invalid directory name. Exception: " + e.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception. Exception: " + e.ToString());
            }         
        }
    }

}
