using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace client_server
{
    class Museum 
    {
        private static MySqlConnection conn;
        private static MySqlDataReader reader;
        private static MySqlCommand command;
        public static void CreateGeoLocationFile()
        {
            conn = Database.GetConnection();
            createJsonFile();
        }

        private static void ExecuteQuery(String query, String[] parameters)
        {
            if(query == null)
            {
                Console.WriteLine("query is null");
                return;
            }
            conn = Database.GetConnection();
            command = new MySqlCommand(query, conn);

            if (parameters != null)
            {
                String parameter = "@val";
                for (int i = 0; i < parameters.Length; i++)
                {
                    command.Parameters.AddWithValue(parameter + (i + 1), parameters[i]);
                }
            
                command.Prepare();
            }

            reader = command.ExecuteReader();
                     
        }

        private static void createJsonFile()
        {
            StringBuilder jsonInfo = new StringBuilder("{ \"museums\": { \"museum\": [ ");
            ExecuteQuery("SELECT name, latitude, longitude, radius FROM SmartMuseumDB.Museums", null);
            while (reader.Read())
            {
                jsonInfo.Append("{ \"name\": \"" + reader[0] + "\"," +
                                "\"latitude\": \"" + reader[1] + "\"," +
                                "\"longitude\": \"" + reader[2] + "\"," +
                                "\"radius\": \"" + reader[3] + "\" },");
                Console.WriteLine(reader[0] + " " + reader[1] + " " + reader[2] + " " + reader[3]);
            }
            jsonInfo.Remove(jsonInfo.Length - 1, 1);
            jsonInfo.Append("] } }");
            reader.Close();
            
            using (FileStream fileStream = File.Create("geoLocations.json"))
            {
                Byte[] info = new UTF8Encoding(true).GetBytes(jsonInfo.ToString());
                fileStream.Write(info, 0, info.Length);
            }
         }

        public static String GetExhibitList(String museum) // probabil nu o sa mai trebuiasca
        {
            if(museum == null)
            {
                Console.WriteLine("Museum is null");
                return null;
            }
            StringBuilder itemList = new StringBuilder();
            ExecuteQuery("SELECT e.name " +
                        " FROM SmartMuseumDB.Museums m INNER JOIN SmartMuseumDB.Exhibits e ON m.id = e.idMuseum " +
                        " WHERE museumName = @val1", new String[] { museum });
            while (reader.Read())
            {
                itemList.Append(reader[0] + ",");
            }
            itemList.Remove(itemList.Length - 1, 1);
            reader.Close();
            Console.WriteLine(itemList.ToString());

            return itemList.ToString();
        }

        public static byte[] GetPackage(String tableName, String queryParameter)
        {
            if(tableName == null || queryParameter == null)
            {
                Console.WriteLine("Table name or query parameter is null");
                return null;
            }
            byte[] byteArrayFile;   
            ExecuteQuery("SELECT path FROM " + tableName + " WHERE name = @val1", new String[] { queryParameter });
            reader.Read();
            Console.WriteLine(reader[0]);    
            byteArrayFile = System.IO.File.ReadAllBytes("E:\\Dropbox\\Facultate\\IP\\Proiect\\Client_Server\\Tablou_de_test.zip"); //de inlocuit cu reader[0]
            //byteArrayFile = System.IO.File.ReadAllBytes("E:\\Dropbox\\Facultate\\IP\\Proiect\\Client_Server\\muzeu_de_test.zip"); //de inlocuit cu reader[0]
            return byteArrayFile;
        }

        /* Probabil vor fii sterse
        public static byte[] GetExhibit(String exhibit)
        {
            byte[] byteArrayFile;
            ExecuteQuery("SELECT path FROM SmartMuseumDB.Exhibits WHERE name = '" + exhibit + "'");
            reader.Read();
            byteArrayFile = System.IO.File.ReadAllBytes("E:\\Dropbox\\Facultate\\IP\\Proiect\\Client_Server\\Tablou_de_test.zip"); //de inlocuit cu reader[0]
            return byteArrayFile;
        }

        public static byte[] GetMuseum(String museum)
        {
            byte[] byteArrayFile;
            ExecuteQuery("SELECT path FROM SmartMuseumDB.Museums where museumName = '" + museum + "'");
            reader.Read();
            byteArrayFile = System.IO.File.ReadAllBytes("E:\\Dropbox\\Facultate\\IP\\Proiect\\Client_Server\\muzeu_de_test.zip"); //de inlocuit cu reader[0]
            return byteArrayFile;
        }
        */
    }
}
