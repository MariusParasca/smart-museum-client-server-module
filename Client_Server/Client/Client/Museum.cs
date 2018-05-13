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
            this.name = name;
            this.outStream = outStream;
            this.inStream = inStream;
            exhibits = new List<Exhibit>();
            //GetMuseum(name);
        }

        public Museum(String name)
        {
            CreateExhibits(name);
        }

        public void GetMuseum(String name) // modifica in privat
        {
            Client.SendText(outStream, name);
            Packet museumPackage = Client.Receive(inStream);
            using (FileStream fs = File.Create("..\\..\\..\\Tablou_de_test.zip")) //de inlocuit cu name(adica numele muzeului);
            {
                //fs.Write(museumPackage, 0, museumPackage.data.Length);
            }
            //trebuie despachetat  zip-ul si apoi apelata createExhibits(path-ul folderului)
        }

        public void CreateExhibits(String museumFolder) // modifica in privat
        {
            string[] directoryEntries = Directory.GetDirectories(museumFolder);
            foreach (string directoryName in directoryEntries)
            {
                Console.WriteLine(directoryName);
                Exhibit exhibit = new Exhibit(directoryName);
                exhibits.Add(exhibit);
            }
                
        }
    }
}
