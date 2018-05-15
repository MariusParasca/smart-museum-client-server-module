using Microsoft.VisualStudio.TestTools.UnitTesting;
using Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Tests
{
    [TestClass()]
    public class MuseumTests
    {
        [TestMethod()]
        public void MuseumTest()
        {
            Museum museum = new Museum(null, null, null);
            Client.connectToServer("127.0.0.1", 8001);
            //museum = new Museum(Client.GetBinaryWriter(), Client.GetBinaryReader(), "//invalid");
            //museum = new Museum(Client.GetBinaryWriter(), Client.GetBinaryReader(), "E:\\Dropbox\\Facultate\\IP\\Proiect\\Client_Server\\Client\\muzeu_de_test");
            museum = new Museum(null);
            museum = new Museum("//invalid");
            museum = new Museum("E:\\Dropbox\\Facultate\\IP\\Proiect\\Client_Server\\Client\\muzeu_de_test");
        }
    }
}