using Microsoft.VisualStudio.TestTools.UnitTesting;
using client_server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace client_server.Tests
{
    [TestClass()]
    public class MuseumTests
    {
        [TestMethod()]
        public void CreateGeoLocationFileTest()
        {
            Museum.CreateGeoLocationFile();
        }

        [TestMethod()]
        public void GetPackageTest()
        {
            Museum.GetPackage(null, null);
            Museum.GetPackage("SmartMuseumDB.Exhibits", "Test");
            Museum.GetPackage("SmartMuseumDB.invalid", "Test");
            Museum.GetPackage("SmartMuseumDB.invalid", "invalid");
            Museum.GetPackage("SmartMuseumDB.Exhibits", "invalid");
        }
    }
}