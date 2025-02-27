using System.Security.Claims;

namespace ELearningPlatform.Client.Helpers
{
    public static class ClaimsPrincipalExtensions
    {
        public static string? GetToken(this ClaimsPrincipal user)
        {
            return user.FindFirst("access_token")?.Value;
        }
    }
}
