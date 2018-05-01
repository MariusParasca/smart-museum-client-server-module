using System;
using MySql.Data.MySqlClient;

namespace client_server
{
    class Database
    {
        private MySqlConnection conn;
        private MySqlCommand command;
        private MySqlDataReader reader;

        public Database() {
            conn = new MySqlConnection();
        }

        private bool OpenConnection()
        {
            conn.ConnectionString = "Server=smartmuseumdb.cye3478n2lhi.eu-west-2.rds.amazonaws.com;" +
                                    "Database=SmartMuseumDB;Uid=masterUser;Pwd=SmartMuseum4Secret;";
            try
            {
                conn.Open();
                return true;
            } catch(MySqlException e)
            {
                Console.WriteLine("Error: " + e.ErrorCode);
                return false;
            }
        }

        public void ExecuteQuery(String query)
        {
            if(OpenConnection() == true)
            {
                command = new MySqlCommand(query, conn);
                reader = command.ExecuteReader();
            }   
        }

        public String GetPath(String museum)
        {
            String query = "SELECT path FROM SmartMuseumDB.Museums WHERE museumName LIKE '" + museum + "%'";
            ExecuteQuery(query);
            reader.Read();
            return (String) reader[0];
        }

        public void PrintQueryResult()
        {
            using(reader)
            {
                while(reader.Read())
                {
                    Console.WriteLine(String.Format("{0} \t | {1} \t | {2} \t | {3} \t | {4} \t | {5}",
                    reader[0], reader[1], reader[2], reader[3], reader[4], reader[5]));
                }
            }
        }
    }
}
