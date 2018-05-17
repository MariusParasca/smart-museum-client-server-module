using Microsoft.VisualStudio.TestTools.UnitTesting;
using Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Client.Tests
{
    public class ClientTests
    {
        [TestMethod()]

        public void MainTest()
        {
            try
            {
                //Client.Main();
            }
            catch (Exception e)
            {

                //    Assert.Fail();
            }

        }

        /*   [TestMethod()]
           public void MainTest1()
           {
               Client.Main();
           }

           [TestMethod()]
           public void ReceiveTextTest()
           {
               try{

                   Client.ReceiveText();

               }
               catch (Exception e){
                   Console.WriteLine("Exceptie " + e.ToString());
               }
           }

           [TestMethod()]
           public void ReceivePhotoTest()
           {

               try
               {

                   Client.ReceivePhoto(" test11.jpg");

               }
               catch (Exception e)
               {
                   Console.WriteLine("Exceptie " + e.ToString());
               }
           }

       /*    [TestMethod()]
           public void ReceiveTest()
           {
               throw new NotImplementedException();
           }

           [TestMethod()]
           public void bArrayToStringTest()
           {
               throw new NotImplementedException();
           }

           [TestMethod()]
           public void SendTextTest()
           {
               throw new NotImplementedException();
           }

           [TestMethod()]
           public void SendPhotoTest()
           {
               throw new NotImplementedException();
           }

           [TestMethod()]
           public void packetToBytesTest()
           {
               throw new NotImplementedException();
           }*/
    }
}