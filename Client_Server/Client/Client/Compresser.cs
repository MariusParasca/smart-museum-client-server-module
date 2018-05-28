    using System.IO;
    using System.IO.Compression;
    namespace Client
    {
        class Compresser
        {

            public static void DecompressZip(string zipPath, string destinationPath)
            {
                if (!Directory.Exists(destinationPath))
                {
                    //Asta este pentru Visual Studio
                    ZipFile.ExtractToDirectory(zipPath, destinationPath);
                    //Asta este pentru Unity
                    //ZipUtil.Unzip(zipPath, destinationPath);
                }
            }

            public static byte[] Compress(byte[] data)
            {
                MemoryStream output = new MemoryStream();
                using (DeflateStream dstream = new DeflateStream(output, CompressionLevel.Optimal))
                {
                    dstream.Write(data, 0, data.Length);
                }
                return output.ToArray();
            }

            public static byte[] Decompress(byte[] data)
            {
                MemoryStream input = new MemoryStream(data);
                MemoryStream output = new MemoryStream();
                using (DeflateStream dstream = new DeflateStream(input, CompressionMode.Decompress))
                {
                    dstream.CopyTo(output);
                }
                return output.ToArray();
            }
        }
    }
