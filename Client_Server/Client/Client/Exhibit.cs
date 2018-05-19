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
            this.name = name;
            this.outStream = outStream;
            this.inStream = inStream;
            imagePaths = new List<String>();
            String path = ".\\Resources\\" + name;
            GetExhibit(path);
        }

        public Exhibit(String exhibitFolder)
        {
            if(exhibitFolder == null)
            {
                Console.WriteLine("exhibitFolder is null");
                return;
            }
            imagePaths = new List<String>();
            this.name = exhibitFolder.Split('\\').Last();
            CreateExhibit(exhibitFolder);
        }

        private void GetExhibit(String pathToExhibit)  // Aceasta metoda trebuie modificata
        {
            try
            {
                Client.SendText(name);
                byte[] exhibitPackage = Client.ReceiveZip();
                /*bool ok = Client.CheckPacketError(exhibitPackage);
                if (!ok)
                {
                    Console.WriteLine("Invalid exhibit name");
                }
                else
                {
                    using (FileStream fs = File.Create(pathToExhibit + ".zip"))
                    {
                        fs.Write(exhibitPackage, 0, exhibitPackage.Length);
                    }
                    //trebuie despachetat  zip-ul si apoi apelata CreateExhibit(path-ul folderului)
                    CreateExhibit(pathToExhibit); // folder hardcodat
                    Console.WriteLine("Package(exhibit) received");
                }*/
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
