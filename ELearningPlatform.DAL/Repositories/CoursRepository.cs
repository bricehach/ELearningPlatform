using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using ELearningPlatform.DAL.Models;
using ELearningPlatform.Repositories;

namespace ELearningPlatform.DAL.Repositories
{
    public class CoursRepository : ICoursRepository
    {
        private readonly string _connectionString;

        public CoursRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Cours>> GetAllAsync()
        {
            var coursList = new List<Cours>();

            using (var connection = new SqlConnection(_connectionString)) // ✅ Correction CS0618
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("SELECT * FROM Cours", connection)) // ✅ Correction CS0618
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            coursList.Add(new Cours
                            {
                                Id = reader.GetInt32(0),
                                Titre = reader.GetString(1),
                                Description = reader.GetString(2),
                                ProfesseurId = reader.GetInt32(3)
                            });
                        }
                    }
                }
            }

            return coursList;
        }
    }
}