using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public class TokenInterceptor : DelegatingHandler
{
    private readonly AuthStateProvider _authStateProvider;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TokenInterceptor(AuthStateProvider authStateProvider, IHttpContextAccessor httpContextAccessor)
    {
        _authStateProvider = authStateProvider ?? throw new ArgumentNullException(nameof(authStateProvider));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = GetTokenFromCookie();

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            Console.WriteLine("⚠️ Token expiré, tentative de rafraîchissement...");

            var refreshed = await _authStateProvider.RefreshTokenAsync();
            if (refreshed)
            {
                Console.WriteLine("✅ Token rafraîchi avec succès !");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", GetTokenFromCookie());
                return await base.SendAsync(request, cancellationToken);
            }
            else
            {
                Console.WriteLine("❌ Échec du rafraîchissement du Token. Suppression des cookies et redirection.");
                RemoveAuthCookies();
            }
        }

        return response;
    }

    private string GetTokenFromCookie()
    {
        return _httpContextAccessor.HttpContext?.Request.Cookies["AccessToken"] ?? "";
    }

    private void RemoveAuthCookies()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context != null)
        {
            context.Response.Cookies.Delete("AccessToken");
            context.Response.Cookies.Delete("RefreshToken");
        }
    }
}
