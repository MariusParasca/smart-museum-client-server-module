﻿using System;
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
        BinaryWriter outStream;
        BinaryReader inStream;

        public Museum (BinaryWriter outStream, BinaryReader inStream, String name)
        {
            this.outStream = outStream;
            this.inStream = inStream;
            exhibits = new List<Exhibit>();
            //GetMuseum(name);
        }

        public void GetMuseum(String name) // modifica in privat
        {
            Client.SendText(outStream, name);
            byte[] museumPackage = Client.Receive(inStream);
            using (FileStream fs = File.Create("..\\..\\..\\Tablou_de_test.zip")) //de inlocuit cu name(adica numele muzeului);
            {
                fs.Write(museumPackage, 0, museumPackage.Length);
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
