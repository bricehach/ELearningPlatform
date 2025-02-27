using ELearningPlatform.ELearningPlatform.Client.Services;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ELearningPlatform.Client.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> Login(string email, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", new { Email = email, Password = password });
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> Register(string email, string password, string nom)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", new { Email = email, Password = password, Nom = nom });
            return response.IsSuccessStatusCode;
        }
    }
}
