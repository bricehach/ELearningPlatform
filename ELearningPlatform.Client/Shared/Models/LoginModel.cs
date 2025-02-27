namespace ELearningPlatform.ELearningPlatform.Client.Shared.Models
{
    /// <summary>
    /// Modèle utilisé pour les informations de connexion de l'utilisateur.
    /// </summary>
    public class LoginModel
    {
        /// <summary>
        /// Adresse e-mail de l'utilisateur utilisée pour l'authentification.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Mot de passe de l'utilisateur.
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
}
