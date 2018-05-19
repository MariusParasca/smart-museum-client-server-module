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
            Museum museum = new Museum(".\\Resources\\muzeu_de_test");
            museum = new Museum(null, null, null);
            museum = new Museum(null);
            museum = new Museum("//invalid");
        }
    }
}