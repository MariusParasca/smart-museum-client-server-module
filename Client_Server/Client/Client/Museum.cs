using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Museum
    {
        List<Exhibit> exhibits;

        public static Museum GetMuseum(BinaryWriter outStream, BinaryReader inStream, String name)
        {
            Museum museum = new Museum();
            Client.SendText(outStream, name);
            //byte[] package = Client.Receive(inStream);
            //File.WriteAllBytes(name + ".zip", package);

            return museum;
        }
    }
}
