using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Security.Claims;
using System.Threading.Tasks;
using ELearningPlatform.Shared.Models;

namespace ELearningPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursController : ControllerBase
    {
        private readonly string _connectionString = "Data Source=GOS-VDI308\\TFTIC;Initial Catalog=ELearningPlatform;Integrated Security=True;Trust Server Certificate=True";

        // ✅ Récupérer les cours de l'utilisateur connecté (JWT nécessaire)
        [HttpGet("mes-cours")]
        [Authorize(Roles = "Apprenant,Formateur,Admin")] // Seuls les utilisateurs authentifiés peuvent accéder
        public async Task<IActionResult> GetMesCours()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Récupérer l'ID utilisateur depuis le JWT

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { error = "Utilisateur non authentifié" });

            var coursList = new List<object>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string query = @"
                        SELECT c.id, c.titre, c.description 
                        FROM dbo.Cours c
                        INNER JOIN dbo.Inscriptions i ON c.id = i.cours_id
                        WHERE i.user_id = @user_id";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@user_id", userId);

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                coursList.Add(new
                                {
                                    Id = reader.GetInt32(0),
                                    Titre = reader.GetString(1),
                                    Description = reader.GetString(2)
                                });
                            }
                        }
                    }
                }

                return Ok(coursList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Erreur serveur", details = ex.Message });
            }
        }
    }
}
