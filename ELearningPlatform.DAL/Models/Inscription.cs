using ELearningPlatform.DAL.Models;

namespace ELearningPlatform.ELearningPlatform.DAL.Models
{
    public class Inscription
    {
        public int Id { get; set; }
        public int UtilisateurId { get; set; } // Clé étrangère vers Utilisateur
        public int CoursId { get; set; } // Clé étrangère vers Cours
        public string Statut { get; set; } = string.Empty; // ✅ Correction CS8618

        // Relations
        public Utilisateur Utilisateur { get; set; } = new Utilisateur(); // ✅ Correction CS8618
        public Cours Cours { get; set; } = new Cours(); // ✅ Correction CS8618
    }
}
