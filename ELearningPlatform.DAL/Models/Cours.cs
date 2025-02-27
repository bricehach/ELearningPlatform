using System.Collections.Generic;
using ELearningPlatform.ELearningPlatform.DAL.Models;

namespace ELearningPlatform.DAL.Models
{
    public class Cours
    {
        public int Id { get; set; }
        public string Titre { get; set; } = string.Empty; // ✅ Correction CS8618
        public string Description { get; set; } = string.Empty; // ✅ Correction CS8618
        public int ProfesseurId { get; set; } // Clé étrangère vers Utilisateur

        // Relation avec Professeur
        public Utilisateur Professeur { get; set; } = new Utilisateur(); // ✅ Correction CS8618

        // Relation avec les Modules
        public ICollection<Module> Modules { get; set; } = new List<Module>();

        // Liste des étudiants inscrits (via Inscription)
        public ICollection<Inscription> Inscriptions { get; set; } = new List<Inscription>();
    }
}
