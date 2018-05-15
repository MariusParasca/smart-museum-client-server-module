﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Client
{
    public class Exhibit
    {
        private List<String> imagePaths;
        private String name = null;
        private String pathToAudioFile = null;
        private ExhibitInfo jsonInfo;
        private BinaryWriter outStream;
        private BinaryReader inStream;

        public Exhibit(BinaryWriter outStream, BinaryReader inStream, String name)
        {
            if (name == null || outStream == null || inStream == null)
            {
                Console.WriteLine("name or outStream or inStream is null");
                return;
            }
            this.outStream = outStream;
            this.inStream = inStream;
            imagePaths = new List<String>();
            GetExhibit(name);
        }

        public Exhibit(String exhibitFolder)
        {
            if(exhibitFolder == null)
            {
                Console.WriteLine("exhibitFolder is null");
                return;
            }
            imagePaths = new List<String>();
            CreateExhibit(exhibitFolder);
        }

        private void GetExhibit(String pathToExhibit)  // Aceasta metoda trebuie modificata
        {
            try
            {
                Client.SendText(name);
                Packet exhibitPackage = Client.Receive();
                using (FileStream fs = File.Create("..\\..\\..\\Tablou_de_test.zip")) //de inlocuit cu name(adica numele exponatului);
                {
                    byte[] packetBytes = Client.packetToBytes(exhibitPackage);
                    //   fs.Write(packetBytes, 0, exhibitPackage.data.Length);
                }
                //trebuie despachetat  zip-ul si apoi apelata CreateExhibit(path-ul folderului)
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception. Exception: " + e.ToString());
            }
        }

        private void CreateExhibit(String exhibitFolder) // modifica in privat
        {
            try
            {
                string[] fileEntries = Directory.GetFiles(exhibitFolder);
                string[] directoryEntries = Directory.GetDirectories(exhibitFolder);
                this.name = exhibitFolder.Split('\\').Last();
                this.pathToAudioFile = fileEntries[0];
                this.LoadJson(fileEntries[1]);
                Console.WriteLine(exhibitFolder.Split('\\').Last() + " " + fileEntries[0] + " " + fileEntries[1]);
                AddImagePaths(directoryEntries[0]);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception. Exception: " + e.ToString());
            }
        }

        private void AddImagePaths(String imgDirectory) // modifica in privat
        {
            try
            {
                string[] fileEntries = Directory.GetFiles(imgDirectory);
                foreach (String filename in fileEntries)
                {
                    this.AddImagePath(filename);
                    Console.WriteLine(filename);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception. Exception: " + e.ToString());
            }

        }

        private void AddImagePath(String imagePath) // modifica in privat
        {
            imagePaths.Add(imagePath);
        }

        public String GetName()
        {
            return name;
        }

        public String GetPathToAudioFile()
        {
            return pathToAudioFile;
        }

        public List<String> GetImagePaths()
        {
            return imagePaths;
        }

        class ExhibitInfo
        {
            public string title { get; set; }
            public string descriptionRo { get; set; }
            public string descriptionEn { get; set; }
            public string linkVideo { get; set; }
        }

        private void LoadJson(String filePath) // modifca in privat
        {
            try
            {
                using (StreamReader r = new StreamReader(filePath))
                {
                    string json = r.ReadToEnd();
                    jsonInfo = JsonConvert.DeserializeObject<ExhibitInfo>(json);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception. Exception: " + e.ToString());
            }
        }

        /*
        public void show()
        {
            Console.WriteLine("[JSON] Titlu:" + jsonInfo.title + "\n");
            Console.WriteLine("[JSON] DescriereRo:" + jsonInfo.descriptionRo + "\n");
            Console.WriteLine("[JSON] DescriereEn:" + jsonInfo.descriptionEn + "\n");
            Console.WriteLine("[JSON] Link Video:" + jsonInfo.linkVideo + "\n");
        }
        */
        public String GetTitle()
        {
            if (jsonInfo == null)
            {
                return null;
            }
            return jsonInfo.title;
        }

        public String GetDescriptionRo()
        {
            if (jsonInfo == null)
            {
                return null;
            }

            return jsonInfo.descriptionRo;
        }

        public String GetDescriptionEn()
        {
            if (jsonInfo == null)
            {
                return null;
            }

            return jsonInfo.descriptionEn;
        }

        public String GetLinkVideo()
        {
            if (jsonInfo == null)
            {
                return null;
            }

            return jsonInfo.linkVideo;
        }

    }

}
