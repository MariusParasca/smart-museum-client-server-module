using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace client_server
{
    public class Package
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
            try
            {
                conn = Database.GetConnection();
                command = new MySqlCommand(query, conn);

                if(parameters != null)
                {
                    String parameter = "@val";
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        command.Parameters.AddWithValue(parameter + (i + 1), parameters[i]);
                    }
                }

                command.Prepare();
                
                reader = command.ExecuteReader();
            }
            catch(MySqlException e)
            {
                Console.WriteLine(e.ToString());
            }

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
            CloseConnection();
            try
            {
                using (FileStream fileStream = File.Create(".//Resources//geoLocations.json"))
                {
                    Byte[] info = new UTF8Encoding(true).GetBytes(jsonInfo.ToString());
                    fileStream.Write(info, 0, info.Length);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            
         }

        public static String GetExhibitList(String museum)
        {
            if(museum == null)
            {
                Console.WriteLine("Museum is null");
                return null;
            }
            StringBuilder itemList = new StringBuilder();
            ExecuteQuery("SELECT e.name " +
                        " FROM SmartMuseumDB.Museums m INNER JOIN SmartMuseumDB.Exhibits e ON m.id = e.idMuseum " +
                        " WHERE m.name = @val1", new String[] { museum });
            while (reader.Read())
            {
                itemList.Append(reader[0] + ",");
            }
            itemList.Remove(itemList.Length - 1, 1);
            Console.WriteLine(itemList.ToString());
            CloseConnection();
            return itemList.ToString();
        }
        private static String GetSingleResult()
        {
            if (reader != null)
            {
                reader.Read();
                if (reader.HasRows)
                {
                    String result = reader[0].ToString();
                    Console.WriteLine(result);
                    return result;
                }
            }
            return "noExist";
        }

        public static String GetPath(String tableName, String queryParameter)
        {
            if (tableName == null || queryParameter == null)
            {
                Console.WriteLine("Table name or query parameter is null");
                return null;
            }
            queryParameter = queryParameter.Replace("\0", String.Empty);
            byte[] byteArrayFile = new byte[] { };
            ExecuteQuery("SELECT path FROM " + tableName + " WHERE name = @val1", new String[] { queryParameter });
            String path = GetSingleResult();
            CloseConnection();
            return path;
        }

        public static bool Login(String username, String password)
        {
            if (username == null || password == null)
            {
                Console.WriteLine("Username or password is null");
                return false;
            }
            username = username.Replace("\0", String.Empty);
            password = password.Replace("\0", String.Empty);
            ExecuteQuery("SELECT id FROM SmartMuseumDB.users WHERE username = @val1 AND password = @val2",
                         new String[] { username, password });
            if(reader != null)
            {
                reader.Read();
                if (reader.HasRows)
                {
                    CloseConnection();
                    return true;
                }
                else
                {
                    CloseConnection();
                    return false;
                }
            }
            CloseConnection();
            return false;
        }

        public static void Register(String username, String password)
        {
            if (username == null || password == null)
            {
                Console.WriteLine("Username or password is null");
                return;
            }
            username = username.Replace("\0", String.Empty);
            password = password.Replace("\0", String.Empty);
            ExecuteQuery("INSERT INTO SmartMuseumDB.users VALUES(null, @val1, @val2)",
                         new String[] { username, password });
            CloseConnection();
        }

        private static string GetMuseumId(String name)
        {
            if (name == null)
            {
                Console.WriteLine("name");
                return "";
            }

            name = name.Replace("\0", String.Empty);
            ExecuteQuery("SELECT id FROM SmartMuseumDB.Museums WHERE name = @val1", new String[] { name });
            String museumId = GetSingleResult();
            CloseConnection();
            return museumId;
        }

        public static void InsertExhibits(String museumName, String author, 
                                          String[] exhibits, String[] paths)
        {
            if (museumName == null || exhibits == null)
            {
                Console.WriteLine("museumName or exhibits is null");
                return;
            }
            if(exhibits.Length == 0)
            {
                Console.WriteLine("Exhibits array is empty");
                return;
            }
            if(exhibits.Length != paths.Length)
            {
                Console.WriteLine("There are not the same number of elements in the arrays");
                return;
            }

            museumName = museumName.Replace("\0", String.Empty);
            String museumId = GetMuseumId(museumName);
            if(museumId.Equals("noExist"))
            {
                Console.WriteLine("Invalid museum name");
                return;
            }
            for(int i = 0; i < exhibits.Length; i++)
            {
                ExecuteQuery(
                    "INSERT INTO SmartMuseumDB.Exhibits VALUES(null, @val1, @val2, @val3, @val4)",
                    new String[] { museumId, author, museumName, paths[i] });
                CloseConnection();
            }
        }

        public static void InsertMuseum(String name, double latitude, 
                                        double longitude, double radius, String path)
        {
            if (name == null || path == null)
            {
                Console.WriteLine("name or path is null");
                return;
            }
            ExecuteQuery("INSERT INTO SmartMuseumDB.Museums VALUES(null, @val1, @val2, @val3, @val4, @val5)",
                         new String[] { name, latitude.ToString(), longitude.ToString(), radius.ToString(), path });
            CloseConnection();
        }


        private static void CloseConnection()
        {
            reader.Close();
            Database.CloseConnection();
        }
        /*
        public static byte[] GetPackage(String tableName, String queryParameter)
        {
            if(tableName == null || queryParameter == null)
            {
                Console.WriteLine("Table name or query parameter is null");
                return null;
            }
            queryParameter = queryParameter.Replace("\0", String.Empty);
            byte[] byteArrayFile = new byte[] {};   
            ExecuteQuery("SELECT path FROM " + tableName + " WHERE name = @val1", new String[] { queryParameter });
            if(reader != null)
            {
                reader.Read();
                if(reader.HasRows)
                {
                    Console.WriteLine(reader[0]);
                    byteArrayFile = System.IO.File.ReadAllBytes(reader[0].ToString());
                }
            }
            CloseConnection();
            return byteArrayFile;
        }


         //Probabil vor fii sterse
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
