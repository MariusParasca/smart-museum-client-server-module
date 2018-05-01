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
    class Exhibit
    {
        private List<Bitmap> images;
        private String pathToAudioFile;
        private String pathToJsonFile;
        private ExhibitInfo jsonInfo;

        class ExhibitInfo
        {
            internal String titlu;
            internal String descriere;
            internal String linkVideo;
        }

        public void LoadJson()
        {
            using (StreamReader r = new StreamReader("..\\..\\..\\file.json"))
            {
                string json = r.ReadToEnd();
                jsonInfo = JsonConvert.DeserializeObject<ExhibitInfo>(json);
            }
        }

        public void show()
        {
            Console.WriteLine("[JSON] Titlu:" + jsonInfo.titlu + "\n");
            Console.WriteLine("[JSON] Descriere:" + jsonInfo.descriere + "\n");
            Console.WriteLine("[JSON] Link Video:" + jsonInfo.linkVideo + "\n");
        }

        public String GetTitlu()
        {
            if (jsonInfo.titlu != null)
            {
                return jsonInfo.titlu;
            }
            return "";
        }

        public String GetDescriere()
        {
            if (jsonInfo.descriere != null)
            {
                return jsonInfo.descriere;
            }
            return "";
        }

        public String GetLinkVideo()
        {
            if (jsonInfo.linkVideo != null)
            {
                return jsonInfo.linkVideo;
            }
            return "";
        }

    }

}
