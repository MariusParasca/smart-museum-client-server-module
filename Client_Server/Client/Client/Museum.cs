﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class Museum : Package
    {
        List<Exhibit> exhibits;
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
            if (GetPackage("get-museum", path, null))
            {
                CreateExhibits(path);
            }
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