using System;
using Microsoft.Data.SqlClient;

namespace ELearningPlatform.DAL.Helpers
{
    public static class DatabaseHelper
    {
        public static void TestDatabaseConnection(string connectionString)
        {
            using var connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                Console.WriteLine("✅ Connexion à la base de données réussie !");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur de connexion à la base de données : {ex.Message}");
            }
        }
    }
}
