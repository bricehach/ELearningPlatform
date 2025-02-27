using System;

namespace ELearningPlatform.DAL.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty; // ✅ Correction CS8618
        public DateTime ExpiryDate { get; set; }
        public int UtilisateurId { get; set; }
        public Utilisateur Utilisateur { get; set; } = new Utilisateur(); // ✅ Correction CS8618
    }
}
