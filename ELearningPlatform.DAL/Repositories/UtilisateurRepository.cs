using ELearningPlatform.DAL.Models;
using Microsoft.Data.SqlClient;

namespace ELearningPlatform.DAL.Repositories
{
    public class UtilisateurRepository : IUtilisateurRepository
    {
        private readonly string _connectionString;

        public UtilisateurRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        // ✅ Récupérer tous les utilisateurs
        public async Task<IEnumerable<Utilisateur>> GetAllAsync()
        {
            var utilisateurs = new List<Utilisateur>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("SELECT Id, Nom, UserName, Surnom, Email, MotDePasseHache, Role FROM Utilisateurs", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            utilisateurs.Add(new Utilisateur
                            {
                                Id = reader.GetInt32(0),
                                Nom = reader.GetString(1),
                                UserName = reader.GetString(2),
                                Surnom = reader.GetString(3),
                                Email = reader.GetString(4),
                                MotDePasseHache = reader.GetString(5),
                                Role = reader.GetString(6)
                            });
                        }
                    }
                }
            }

            return utilisateurs;
        }

        // ✅ Récupérer un utilisateur par ID
        public async Task<Utilisateur?> GetByIdAsync(int id)
        {
            Utilisateur? utilisateur = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("SELECT Id, Nom, UserName, Surnom, Email, MotDePasseHache, Role, RefreshToken, RefreshTokenExpiry FROM Utilisateurs WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            utilisateur = new Utilisateur
                            {
                                Id = reader.GetInt32(0),
                                Nom = reader.GetString(1),
                                UserName = reader.GetString(2),
                                Surnom = reader.GetString(3),
                                Email = reader.GetString(4),
                                MotDePasseHache = reader.GetString(5),
                                Role = reader.GetString(6),
                                RefreshToken = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                                RefreshTokenExpiry = reader.IsDBNull(8) ? DateTime.MinValue : reader.GetDateTime(8)
                            };
                        }
                    }
                }
            }

            return utilisateur;
        }

        // ✅ Récupérer un utilisateur par Email
        public async Task<Utilisateur?> GetByEmailAsync(string email)
        {
            Utilisateur? utilisateur = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("SELECT Id, Nom, UserName, Surnom, Email, MotDePasseHache, Role, RefreshToken, RefreshTokenExpiry FROM Utilisateurs WHERE Email = @Email", connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            utilisateur = new Utilisateur
                            {
                                Id = reader.GetInt32(0),
                                Nom = reader.GetString(1),
                                UserName = reader.GetString(2),
                                Surnom = reader.GetString(3),
                                Email = reader.GetString(4),
                                MotDePasseHache = reader.GetString(5),
                                Role = reader.GetString(6),
                                RefreshToken = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                                RefreshTokenExpiry = reader.IsDBNull(8) ? DateTime.MinValue : reader.GetDateTime(8)
                            };
                        }
                    }
                }
            }

            return utilisateur;
        }

        // ✅ Récupérer un utilisateur par RefreshToken
        public async Task<Utilisateur?> GetUserByRefreshToken(string refreshToken)
        {
            Utilisateur? utilisateur = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("SELECT Id, Nom, UserName, Surnom, Email, MotDePasseHache, Role, RefreshToken, RefreshTokenExpiry FROM Utilisateurs WHERE RefreshToken = @RefreshToken", connection))
                {
                    command.Parameters.AddWithValue("@RefreshToken", refreshToken);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            utilisateur = new Utilisateur
                            {
                                Id = reader.GetInt32(0),
                                Nom = reader.GetString(1),
                                UserName = reader.GetString(2),
                                Surnom = reader.GetString(3),
                                Email = reader.GetString(4),
                                MotDePasseHache = reader.GetString(5),
                                Role = reader.GetString(6),
                                RefreshToken = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                                RefreshTokenExpiry = reader.IsDBNull(8) ? DateTime.MinValue : reader.GetDateTime(8)
                            };
                        }
                    }
                }
            }

            return utilisateur;
        }

        // ✅ Ajouter un utilisateur
        public async Task AddAsync(Utilisateur utilisateur)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(
                    "INSERT INTO Utilisateurs (Nom, UserName, Surnom, Email, MotDePasseHache, Role, RefreshToken, RefreshTokenExpiry) VALUES (@Nom, @UserName, @Surnom, @Email, @MotDePasseHache, @Role, @RefreshToken, @RefreshTokenExpiry)", connection))
                {
                    command.Parameters.AddWithValue("@Nom", utilisateur.Nom);
                    command.Parameters.AddWithValue("@UserName", utilisateur.UserName);
                    command.Parameters.AddWithValue("@Surnom", utilisateur.Surnom);
                    command.Parameters.AddWithValue("@Email", utilisateur.Email);
                    command.Parameters.AddWithValue("@MotDePasseHache", utilisateur.MotDePasseHache);
                    command.Parameters.AddWithValue("@Role", utilisateur.Role);
                    command.Parameters.AddWithValue("@RefreshToken", utilisateur.RefreshToken ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@RefreshTokenExpiry", utilisateur.RefreshTokenExpiry);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        // ✅ Mettre à jour un utilisateur
        public async Task UpdateAsync(Utilisateur utilisateur)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(
                    "UPDATE Utilisateurs SET Nom = @Nom, UserName = @UserName, Surnom = @Surnom, Email = @Email, MotDePasseHache = @MotDePasseHache, Role = @Role, RefreshToken = @RefreshToken, RefreshTokenExpiry = @RefreshTokenExpiry WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", utilisateur.Id);
                    command.Parameters.AddWithValue("@Nom", utilisateur.Nom);
                    command.Parameters.AddWithValue("@UserName", utilisateur.UserName);
                    command.Parameters.AddWithValue("@Surnom", utilisateur.Surnom);
                    command.Parameters.AddWithValue("@Email", utilisateur.Email);
                    command.Parameters.AddWithValue("@MotDePasseHache", utilisateur.MotDePasseHache);
                    command.Parameters.AddWithValue("@Role", utilisateur.Role);
                    command.Parameters.AddWithValue("@RefreshToken", utilisateur.RefreshToken ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@RefreshTokenExpiry", utilisateur.RefreshTokenExpiry);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        // ✅ Supprimer un utilisateur
        public async Task DeleteAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("DELETE FROM Utilisateurs WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
