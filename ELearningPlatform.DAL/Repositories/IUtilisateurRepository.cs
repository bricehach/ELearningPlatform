using System.Collections.Generic;
using System.Threading.Tasks;
using ELearningPlatform.DAL.Models;

namespace ELearningPlatform.DAL.Repositories
{
    public interface IUtilisateurRepository
    {
        Task<IEnumerable<Utilisateur>> GetAllAsync(); // ✅ Récupérer tous les utilisateurs
        Task<Utilisateur?> GetByIdAsync(int id); // ✅ Récupérer un utilisateur par son ID
        Task<Utilisateur?> GetByEmailAsync(string email); // ✅ Récupérer un utilisateur par email
        Task<Utilisateur?> GetUserByRefreshToken(string refreshToken); // ✅ Récupérer un utilisateur par refresh token
        Task AddAsync(Utilisateur utilisateur); // ✅ Ajouter un utilisateur
        Task UpdateAsync(Utilisateur utilisateur); // ✅ Mettre à jour un utilisateur
        Task DeleteAsync(int id); // ✅ Supprimer un utilisateur
    }
}
