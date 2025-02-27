namespace ELearningPlatform.DAL.Models
{
    public class Module
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty; // ✅ Correction CS8618
        public string Contenu { get; set; } = string.Empty; // ✅ Correction CS8618
        public int CoursId { get; set; } // Clé étrangère vers Cours

        // Relation avec Cours
        public Cours Cours { get; set; } = new Cours(); // ✅ Correction CS8618
    }
}
