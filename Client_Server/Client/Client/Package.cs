﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public abstract class Package
    {
        protected BinaryWriter outStream;
        protected BinaryReader inStream;
        protected String name = null;

        protected void GetPackage(String packetType, String pathToPackage)
        {
            try
            {
                Client.SendText(packetType, this.name);
                Client.ReceiveZip();
                Compresser.DecompressZip(pathToPackage + ".zip", pathToPackage);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception. Exception: " + e.ToString());
            }

        }

    }
}
