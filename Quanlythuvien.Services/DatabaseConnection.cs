using System;
using Microsoft.Data.SqlClient;

namespace Quanlythuvien.Services
{
    public static class DatabaseConnection
    {
        private const string ServerName = "LAPTOP-EBNPBAJE";
        private const string DatabaseName = "quanlythuvien";

        public static string Server { get { return ServerName; } }
        public static string Database { get { return DatabaseName; } }

        private static string BuildConnectionString(string catalog)
        {
            return "Data Source=" + ServerName + ";Initial Catalog=" + catalog + ";Integrated Security=True;Encrypt=True;TrustServerCertificate=True;Connect Timeout=30;";
        }

        public static SqlConnection Open()
        {
            SqlConnection connection = new SqlConnection(BuildConnectionString(DatabaseName));
            try { connection.Open(); return connection; }
            catch { connection.Dispose(); throw; }
        }

        public static void Initialize()
        {
            using (SqlConnection connection = new SqlConnection(BuildConnectionString("master")))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    // This only creates an absent database; it never alters existing schema or records.
                    command.CommandText = "IF DB_ID(@databaseName) IS NULL CREATE DATABASE [quanlythuvien]";
                    command.Parameters.Add("@databaseName", System.Data.SqlDbType.NVarChar, 128).Value = DatabaseName;
                    command.ExecuteNonQuery();
                }
            }
            using (SqlConnection connection = Open())
            using (SqlCommand command = new SqlCommand("SELECT DB_NAME()", connection))
            {
                if (!string.Equals(Convert.ToString(command.ExecuteScalar()), DatabaseName, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("Không thể kết nối đúng database " + DatabaseName + ".");
            }
        }
    }
}
