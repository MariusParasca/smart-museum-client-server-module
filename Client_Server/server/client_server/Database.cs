using System;
using MySql.Data.MySqlClient;

namespace client_server
{
    class Database
    {
        private static MySqlConnection conn;

        private Database() { }

        public static MySqlConnection GetConnection()
        {
            if (conn == null)
            {
                CreateConnection();
            }

            return conn;
        }

        private static void CreateConnection()
        {
            try
            {
                conn = new MySqlConnection();
                conn.ConnectionString = "Server=smartmuseumdb.cye3478n2lhi.eu-west-2.rds.amazonaws.com;" +
                                           "Database=SmartMuseumDB;Uid=masterUser;Pwd=SmartMuseum4Secret;";
                conn.Open();
            }
            catch (MySqlException e)
            {
                Console.WriteLine("Error: " + e.ErrorCode);

            }
        }

        public static void CloseConnection()
        {
            conn.Close();
            conn = null;
        }
    }
}
