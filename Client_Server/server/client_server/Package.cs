using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Text;


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

        private static void ExecuteQuery(string query, string[] parameters)
        {
            try
            {
                conn = Database.GetConnection();
                command = new MySqlCommand(query, conn);

                if(parameters != null)
                {
                    string parameter = "@val";
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
                    byte[] info = new UTF8Encoding(true).GetBytes(jsonInfo.ToString());
                    fileStream.Write(info, 0, info.Length);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            
         }

        public static string GetExhibitList(string museum)
        {
            if(museum == null)
            {
                Console.WriteLine("Museum is null");
                return null;
            }
            StringBuilder itemList = new StringBuilder();
            ExecuteQuery("SELECT e.name " +
                        " FROM SmartMuseumDB.Museums m INNER JOIN SmartMuseumDB.Exhibits e ON m.id = e.idMuseum " +
                        " WHERE m.name = @val1", new string[] { museum });
            while (reader.Read())
            {
                itemList.Append(reader[0] + ",");
            }
            itemList.Remove(itemList.Length - 1, 1);
            Console.WriteLine(itemList.ToString());
            CloseConnection();
            return itemList.ToString();
        }
        private static string GetSingleResult()
        {
            if (reader != null)
            {
                reader.Read();
                if (reader.HasRows)
                {
                    string result = reader[0].ToString();
                    Console.WriteLine(result);
                    return result;
                }
            }
            return "noExist";
        }

        public static string GetPath(string tableName, string queryParameter)
        {
            if (tableName == null || queryParameter == null)
            {
                Console.WriteLine("Table name or query parameter is null");
                return null;
            }
            queryParameter = queryParameter.Replace("\0", string.Empty);
            byte[] byteArrayFile = new byte[] { };
            ExecuteQuery("SELECT path FROM " + tableName + " WHERE name = @val1", new string[] { queryParameter });
            string path = GetSingleResult();
            CloseConnection();
            return path;
        }

        public static bool Login(string username, string password)
        {
            if (username == null || password == null)
            {
                Console.WriteLine("Username or password is null");
                return false;
            }
            username = username.Replace("\0", string.Empty);
            password = password.Replace("\0", string.Empty);
            ExecuteQuery("SELECT id FROM SmartMuseumDB.users WHERE username = @val1 AND password = @val2",
                         new string[] { username, password });
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

        public static void Register(string username, string password)
        {
            if (username == null || password == null)
            {
                Console.WriteLine("Username or password is null");
                return;
            }
            username = username.Replace("\0", string.Empty);
            password = password.Replace("\0", string.Empty);
            ExecuteQuery("INSERT INTO SmartMuseumDB.users VALUES(null, @val1, @val2)",
                         new string[] { username, password });
            CloseConnection();
        }

        private static string GetMuseumId(string name)
        {
            if (name == null)
            {
                Console.WriteLine("name");
                return "";
            }

            name = name.Replace("\0", string.Empty);
            ExecuteQuery("SELECT id FROM SmartMuseumDB.Museums WHERE name = @val1", new string[] { name });
            string museumId = GetSingleResult();
            CloseConnection();
            return museumId;
        }

        public static void InsertExhibits(string museumName, string author, 
                                          string[] exhibits, string[] paths)
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

            museumName = museumName.Replace("\0", string.Empty);
            string museumId = GetMuseumId(museumName);
            if(museumId.Equals("noExist"))
            {
                Console.WriteLine("Invalid museum name");
                return;
            }
            for(int i = 0; i < exhibits.Length; i++)
            {
                ExecuteQuery(
                    "INSERT INTO SmartMuseumDB.Exhibits VALUES(null, @val1, @val2, @val3, @val4)",
                    new string[] { museumId, author, museumName, paths[i] });
                CloseConnection();
            }
        }

        public static void InsertMuseum(string name, double latitude, 
                                        double longitude, double radius, string path)
        {
            if (name == null || path == null)
            {
                Console.WriteLine("name or path is null");
                return;
            }
            ExecuteQuery("INSERT INTO SmartMuseumDB.Museums VALUES(null, @val1, @val2, @val3, @val4, @val5)",
                         new string[] { name, latitude.ToString(), longitude.ToString(), radius.ToString(), path });
            CloseConnection();
        }


        private static void CloseConnection()
        {
            reader.Close();
            Database.CloseConnection();
        }
        /*
        public static byte[] GetPackage(string tableName, string queryParameter)
        {
            if(tableName == null || queryParameter == null)
            {
                Console.WriteLine("Table name or query parameter is null");
                return null;
            }
            queryParameter = queryParameter.Replace("\0", string.Empty);
            byte[] byteArrayFile = new byte[] {};   
            ExecuteQuery("SELECT path FROM " + tableName + " WHERE name = @val1", new string[] { queryParameter });
            if(reader != null)
            {
                reader.Read();
                if(reader.HasRows)
                {
                    Console.WriteLine(reader[0]);
                    byteArrayFile = System.IO.File.ReadAllBytes(reader[0].Tostring());
                }
            }
            CloseConnection();
            return byteArrayFile;
        }


         //Probabil vor fii sterse
        public static byte[] GetExhibit(string exhibit)
        {
            byte[] byteArrayFile;
            ExecuteQuery("SELECT path FROM SmartMuseumDB.Exhibits WHERE name = '" + exhibit + "'");
            reader.Read();
            byteArrayFile = System.IO.File.ReadAllBytes("E:\\Dropbox\\Facultate\\IP\\Proiect\\Client_Server\\Tablou_de_test.zip"); //de inlocuit cu reader[0]
            return byteArrayFile;
        }

        public static byte[] GetMuseum(string museum)
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
