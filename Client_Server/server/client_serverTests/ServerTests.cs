using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Tests
{
    [TestClass()]
    public class ServerTests
    {
        [TestMethod()]
        public void MainTest()
        {
           Server.Main();
           // Assert.Fail();
        }
    }
}