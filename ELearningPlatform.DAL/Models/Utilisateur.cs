using ELearningPlatform.ELearningPlatform.DAL.Models;
using System;
using System.Collections.Generic;

namespace ELearningPlatform.DAL.Models
{
    public class Utilisateur
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty; // ✅ Correction CS8618
        public string UserName { get; set; } = string.Empty; // ✅ Correction CS8618
        public string Surnom { get; set; } = string.Empty; // ✅ Correction CS8618
        public string Email { get; set; } = string.Empty; // ✅ Correction CS8618
        public string MotDePasseHache { get; set; } = string.Empty; // ✅ Correction CS8618
        public string Role { get; set; } = string.Empty; // ✅ Correction CS8618

        // ✅ Ajout du Refresh Token pour l'authentification sécurisée
        public string RefreshToken { get; set; } = string.Empty; // ✅ Correction CS8618
        public DateTime RefreshTokenExpiry { get; set; }

        // Relations avec d'autres entités (sans EF)
        public List<Progression> Progressions { get; set; } = new List<Progression>();
        public List<Cours> CoursEnseignes { get; set; } = new List<Cours>();
    }
}
