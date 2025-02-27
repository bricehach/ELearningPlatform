using System;
using Microsoft.Data.SqlClient; // ✅ Utilisation de Microsoft.Data.SqlClient
using Microsoft.Extensions.Configuration;

namespace ELearningPlatform.DAL.Data
{
    public static class DatabaseInitializer
    {
        public static void InitializeDatabase(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("❌ La chaîne de connexion SQL est absente de la configuration.");

            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("✅ Connexion à `ELearningPlatform` réussie !");

                    // ✅ Création des tables si elles n'existent pas
                    ExecuteSqlCommand(connection, "Utilisateurs", @"
                        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name='Utilisateurs')
                        CREATE TABLE Utilisateurs (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            Nom NVARCHAR(100) NOT NULL,
                            UserName NVARCHAR(100) NOT NULL,
                            Surnom NVARCHAR(100) NOT NULL,
                            Email NVARCHAR(255) UNIQUE NOT NULL,
                            MotDePasseHache NVARCHAR(255) NOT NULL,
                            Role NVARCHAR(50) NOT NULL,
                            RefreshToken NVARCHAR(255) NULL,
                            RefreshTokenExpiry DATETIME NULL
                        );");

                    ExecuteSqlCommand(connection, "Cours", @"
                        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name='Cours')
                        CREATE TABLE Cours (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            Titre NVARCHAR(255) NOT NULL,
                            Description NVARCHAR(MAX) NOT NULL,
                            ProfesseurId INT NOT NULL,
                            FOREIGN KEY (ProfesseurId) REFERENCES Utilisateurs(Id) ON DELETE CASCADE
                        );");

                    ExecuteSqlCommand(connection, "Progression", @"
                        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name='Progression')
                        CREATE TABLE Progression (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            UtilisateurId INT NOT NULL,
                            CoursId INT NOT NULL,
                            ProgressionPourcentage FLOAT NOT NULL,
                            FOREIGN KEY (UtilisateurId) REFERENCES Utilisateurs(Id) ON DELETE NO ACTION,
                            FOREIGN KEY (CoursId) REFERENCES Cours(Id) ON DELETE CASCADE
                        );");

                    ExecuteSqlCommand(connection, "Evaluations", @"
                        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name='Evaluations')
                        CREATE TABLE Evaluations (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            Titre NVARCHAR(255) NOT NULL,
                            Type NVARCHAR(50) NOT NULL,
                            CoursId INT NOT NULL,
                            FOREIGN KEY (CoursId) REFERENCES Cours(Id) ON DELETE CASCADE
                        );");

                    ExecuteSqlCommand(connection, "Modules", @"
                        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name='Modules')
                        CREATE TABLE Modules (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            Nom NVARCHAR(255) NOT NULL,
                            Contenu NVARCHAR(MAX) NOT NULL,
                            CoursId INT NOT NULL,
                            FOREIGN KEY (CoursId) REFERENCES Cours(Id) ON DELETE CASCADE
                        );");

                    ExecuteSqlCommand(connection, "Inscriptions", @"
                        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name='Inscriptions')
                        CREATE TABLE Inscriptions (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            UtilisateurId INT NOT NULL,
                            CoursId INT NOT NULL,
                            Statut NVARCHAR(50) NOT NULL,
                            FOREIGN KEY (UtilisateurId) REFERENCES Utilisateurs(Id) ON DELETE CASCADE,
                            FOREIGN KEY (CoursId) REFERENCES Cours(Id) ON DELETE CASCADE
                        );");

                    Console.WriteLine("✅ Toutes les tables ont été créées avec succès !");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Erreur lors de la création des tables : {ex.Message}");
                }
            }
        }

        private static void ExecuteSqlCommand(SqlConnection connection, string tableName, string sqlCommand)
        {
            try
            {
                using (var command = new SqlCommand(sqlCommand, connection))
                {
                    command.ExecuteNonQuery();
                }
                Console.WriteLine($"✅ Table `{tableName}` créée ou déjà existante.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur lors de la création de `{tableName}` : {ex.Message}");
            }
        }
    }
}
