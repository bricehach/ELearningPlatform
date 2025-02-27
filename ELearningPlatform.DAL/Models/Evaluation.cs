using ELearningPlatform.DAL.Models;

namespace ELearningPlatform.ELearningPlatform.DAL.Models
{
    public class Evaluation
    {
        public int Id { get; set; }
        public string Titre { get; set; } = string.Empty; // ✅ Correction CS8618
        public string Type { get; set; } = string.Empty; // ✅ Correction CS8618
        public int CoursId { get; set; } // Clé étrangère vers Cours

        // Relation avec Cours
        public Cours Cours { get; set; } = new Cours(); // ✅ Correction CS8618
    }
}
