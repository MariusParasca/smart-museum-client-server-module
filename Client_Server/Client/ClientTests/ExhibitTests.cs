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
            Client.connectToServer("127.0.0.1", 8001);
            exhibit = new Exhibit(Client.GetBinaryWriter(), Client.GetBinaryReader(), "//fasfa.fsdfs3/';[]fsda");
            exhibit = new Exhibit(Client.GetBinaryWriter(), null, "ceva");
            exhibit = new Exhibit(Client.GetBinaryWriter(), Client.GetBinaryReader(), null);
            exhibit.GetDescriptionEn();
            exhibit.GetDescriptionRo();
            exhibit.GetImagePaths();
            exhibit.GetTitle();
            exhibit.GetName();
            exhibit.GetLinkVideo();
            exhibit.GetPathToAudioFile();
            /*
            exhibit.AddImagePath(null);
            exhibit.AddImagePaths(null);
            exhibit.GetExhibit(null);
            exhibit.CreateExhibit(null);
            exhibit.LoadJson(null);
            exhibit.AddImagePath("//invalid");
            exhibit.AddImagePaths("//invalid");
            exhibit.GetExhibit("//invalid");
            exhibit.CreateExhibit("//invalid");
            exhibit.LoadJson("//invalid");
            */
            //Assert.Fail();
        }

        [TestMethod()]
        public void LoadExhibitTest()
        {
            Client.connectToServer("127.0.0.1", 8001);
            Exhibit exhibit = new Exhibit("E:\\Dropbox\\Facultate\\IP\\Proiect\\Client_Server\\Client\\muzeu_de_test\\Tablou_de_test");
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