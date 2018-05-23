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
            Package.CreateGeoLocationFile();
        }

        [TestMethod()]
        public void GetPackageTest()
        {
           /* Package.GetPackage(null, null);
            Package.GetPackage("SmartMuseumDB.Exhibits", "Test");
            Package.GetPackage("SmartMuseumDB.invalid", "Test");
            Package.GetPackage("SmartMuseumDB.invalid", "invalid");
            Package.GetPackage("SmartMuseumDB.Exhibits", "invalid");
            */
        }
    }
}