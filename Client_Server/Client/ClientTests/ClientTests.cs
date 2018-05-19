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
    [TestClass()]
    public class ClientTests
    {
        [TestMethod()]

        public void MainTest()
        {
            try
            {
                Client.Main();
              

                
            }
            catch (Exception e)
            {

                Console.WriteLine("Exceptie " + e.ToString());
            }

        }
     /*
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

                Client.ReceivePhoto("test.jpg");

            }
               catch (Exception e)
               {
                   Console.WriteLine("Exceptie " + e.ToString());
               }
           }
        [TestMethod()]
        public void SendTextTest()
        {

            try
            {

                 Client.SendText("String de test");

            }
            catch (Exception e)
            {
                Console.WriteLine("Exceptie " + e.ToString());
            }
        }
        [TestMethod()]
        public void SendPhotoTest()
        {

            try
            {

                Client.SendPhoto(".\\Resources\\test.jpg");

            }
            catch (Exception e)
            {
                Console.WriteLine("Exceptie " + e.ToString());
            }
        }
        */
    }
}