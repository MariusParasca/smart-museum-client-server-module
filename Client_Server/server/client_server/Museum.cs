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
        public static void CreateGeoLocaitonFile()
        {
            conn = Database.GetConnection();
            createJsonFile();
        }

        private static void ExecuteQuery(String query)
        {
            command = new MySqlCommand(query, conn);

            reader = command.ExecuteReader();   
        }

        private static void createJsonFile()
        {
            StringBuilder jsonInfo = new StringBuilder("{ \"museums\": { \"museum\": [ ");
            ExecuteQuery("SELECT museumName, latitude, longitude, radius FROM SmartMuseumDB.Museums");
            while (reader.Read())
            {
                jsonInfo.Append("{ \"museumName\": \"" + reader[0] + "\"," +
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

    }
}
