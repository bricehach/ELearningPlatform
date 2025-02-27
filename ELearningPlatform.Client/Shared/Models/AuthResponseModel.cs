namespace ELearningPlatform.ELearningPlatform.Client.Shared.Models
{
    /// <summary>
    /// Modèle de réponse d'authentification contenant les tokens JWT.
    /// </summary>
    public class AuthResponseModel
    {
        /// <summary>
        /// Token d'accès JWT permettant l'authentification de l'utilisateur.
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// Token de rafraîchissement permettant de générer un nouveau token d'accès.
        /// </summary>
        public string RefreshToken { get; set; } = string.Empty;
    }
}
