using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Client
{
    public class Exhibit : Package
    {
        private List<String> imagePaths;
        private String pathToAudioFile = null;
        private ExhibitInfo jsonInfo;
        
        public Exhibit(BinaryWriter outStream, BinaryReader inStream, String museumName, String exhibitName) // trebuie modificata
        {
            if (exhibitName == null || museumName == null || outStream == null || inStream == null)
            {
                Console.WriteLine("name or outStream or inStream is null");
                return;
            }
            this.name = exhibitName;
            this.outStream = outStream;
            this.inStream = inStream;
            imagePaths = new List<String>();
            //String path = ".\\Resources\\" + name;
            CreateFolder(".\\Resources\\" + museumName);
            String path = ".\\Resources\\" + museumName + "\\" + exhibitName;
            GetPackage("get-exhibit", path, museumName);
            CreateExhibit(path);
            if(GetPackage("get-exhibit", path, museumName))
            {
                CreateExhibit(path);
            }
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

        private void CreateFolder(String path)
        {
            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private void CreateExhibit(String exhibitFolder) 
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

        private void AddImagePaths(String imgDirectory)
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

        private void AddImagePath(String imagePath)
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
