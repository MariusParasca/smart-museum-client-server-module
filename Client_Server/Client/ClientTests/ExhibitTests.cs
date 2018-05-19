using Microsoft.VisualStudio.TestTools.UnitTesting;
using Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Sockets;

namespace Client.Tests
{
    [TestClass()]
    public class ExhibitTests
    {
        [TestMethod()]
        public void ExhibitTest()
        {
            Exhibit exhibit = new Exhibit(null, null, null);
            exhibit = new Exhibit(null);
            exhibit = new Exhibit("//invalid");
            exhibit = new Exhibit(null, null, "ceva");
            exhibit.GetDescriptionEn();
            exhibit.GetDescriptionRo();
            exhibit.GetImagePaths();
            exhibit.GetTitle();
            exhibit.GetName();
            exhibit.GetLinkVideo();
            exhibit.GetPathToAudioFile();
        }

        [TestMethod()]
        public void LoadExhibitTest()
        {
            Exhibit exhibit = new Exhibit(".\\Resources\\muzeu_de_test\\Tablou_de_test");
            exhibit.GetDescriptionEn();
            exhibit.GetDescriptionRo();
            exhibit.GetImagePaths();
            exhibit.GetTitle();
            exhibit.GetName();
            exhibit.GetLinkVideo();
            exhibit.GetPathToAudioFile();
        }
    }
}