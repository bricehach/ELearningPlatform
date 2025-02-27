namespace ELearningPlatform.ELearningPlatform.Client.Shared.Models
{
    /// <summary>
    /// Modèle utilisé pour l'inscription d'un nouvel utilisateur.
    /// </summary>
    public class RegisterModel
    {
        /// <summary>
        /// Nom de l'utilisateur.
        /// </summary>
        public string Nom { get; set; } = string.Empty;

        /// <summary>
        /// Adresse e-mail de l'utilisateur utilisée pour l'inscription.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Mot de passe choisi par l'utilisateur.
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
}
