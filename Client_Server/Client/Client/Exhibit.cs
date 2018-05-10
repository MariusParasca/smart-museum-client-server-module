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
            public string title { get; set; }
            public string descriptionRo { get; set; }
            public string descriptionEn { get; set; }
            public string linkVideo { get; set; }
        }

        public void LoadJson(String filePath)
        {
            using (StreamReader r = new StreamReader(filePath))
            {
                string json = r.ReadToEnd();
                jsonInfo = JsonConvert.DeserializeObject<ExhibitInfo>(json);
            }
        }

        public void show()
        {
            Console.WriteLine("[JSON] Titlu:" + jsonInfo.title + "\n");
            Console.WriteLine("[JSON] DescriereRo:" + jsonInfo.descriptionRo + "\n");
            Console.WriteLine("[JSON] DescriereEn:" + jsonInfo.descriptionEn + "\n");
            Console.WriteLine("[JSON] Link Video:" + jsonInfo.linkVideo + "\n");
        }

        public String GetTitle()
        {
            if (jsonInfo.title != null)
            {
                return jsonInfo.title;
            }
            return "";
        }

        public String GetDescriptionRo()
        {
            if (jsonInfo.descriptionRo != null)
            {
                return jsonInfo.descriptionRo;
            }
            return "";
        }

        public String GetDescriptionEn()
        {
            if (jsonInfo.descriptionEn != null)
            {
                return jsonInfo.descriptionEn;
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
