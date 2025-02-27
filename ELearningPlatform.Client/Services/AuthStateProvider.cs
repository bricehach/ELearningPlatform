using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;
using ELearningPlatform.Client.Helpers;

public class AuthStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public new event Action<AuthenticationState> AuthenticationStateChanged = delegate { }; // ✅ Correction CS8618, CS0108

    public AuthStateProvider(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = GetTokenFromCookie();
        var identity = string.IsNullOrEmpty(token)
            ? new ClaimsIdentity()
            : new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "jwt");

        return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
    }

    public void SetTokens(string accessToken, string refreshToken)
    {
        _httpContextAccessor.HttpContext?.Response?.Cookies.Append("AccessToken", accessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddMinutes(15)
        });

        _httpContextAccessor.HttpContext?.Response?.Cookies.Append("RefreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        });

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task<bool> RefreshTokenAsync()
    {
        try
        {
            var refreshToken = _httpContextAccessor.HttpContext?.Request?.Cookies?["RefreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                Console.WriteLine("⚠️ Aucun Refresh Token disponible.");
                return false;
            }

            var response = await _httpClient.PostAsJsonAsync("api/auth/refresh", new { RefreshToken = refreshToken });

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("❌ Échec du rafraîchissement du Token.");
                RemoveAuthCookies();
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
                return false;
            }

            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
            if (result != null && !string.IsNullOrEmpty(result.AccessToken))
            {
                SetTokens(result.AccessToken, result.RefreshToken);
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur lors du rafraîchissement du Token : {ex.Message}");
            RemoveAuthCookies();
        }

        return false;
    }

    private string GetTokenFromCookie()
    {
        return _httpContextAccessor.HttpContext?.Request?.Cookies?["AccessToken"] ?? ""; // ✅ Correction CS8602
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

    private new void NotifyAuthenticationStateChanged(Task<AuthenticationState> task) // ✅ Correction CS0108
    {
        AuthenticationStateChanged?.Invoke(task.Result);
    }

    private class AuthResponse
    {
        public string AccessToken { get; set; } = string.Empty; // ✅ Correction CS8618
        public string RefreshToken { get; set; } = string.Empty; // ✅ Correction CS8618
    }
}
