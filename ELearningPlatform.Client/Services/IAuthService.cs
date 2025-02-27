namespace ELearningPlatform.ELearningPlatform.Client.Services
{
    public interface IAuthService
    {
        Task<bool> Login(string email, string password);
        Task<bool> Register(string email, string password, string nom);

    }
}
