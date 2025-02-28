using ELearningPlatform.DAL.Models;
using ELearningPlatform.DAL.Repositories;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ELearningPlatform.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUtilisateurRepository _utilisateurRepository;
        private readonly IConfiguration _configuration;

        public AuthController(IUtilisateurRepository utilisateurRepository, IConfiguration configuration)
        {
            _utilisateurRepository = utilisateurRepository ?? throw new ArgumentNullException(nameof(utilisateurRepository));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        // ✅ Endpoint de connexion sécurisé
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "Email et mot de passe sont obligatoires." });
            }

            var safeEmail = SanitizeInput(request.Email);
            var utilisateur = await _utilisateurRepository.GetByEmailAsync(safeEmail);

            if (utilisateur == null || !VerifyPassword(request.Password, utilisateur.MotDePasseHache))
            {
                Console.WriteLine($"⚠️ Tentative de connexion échouée pour : {safeEmail}");
                return Unauthorized(new { message = "Identifiants incorrects." });
            }

            var accessToken = GenerateAccessToken(utilisateur);
            var refreshToken = GenerateRefreshToken();

            utilisateur.RefreshToken = refreshToken ?? string.Empty; // ✅ Correction CS8625
            utilisateur.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _utilisateurRepository.UpdateAsync(utilisateur);

            // ✅ Stockage des Tokens dans des Cookies sécurisés
            SetSecureCookies(accessToken, refreshToken ?? string.Empty); // ✅ Correction CS8604

            Console.WriteLine($"✅ Connexion réussie : {utilisateur.Email}");
            return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
        }

        // ✅ Endpoint de rafraîchissement sécurisé
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            if (!Request.Cookies.TryGetValue("RefreshToken", out var refreshToken))
            {
                return BadRequest(new { message = "Le Refresh Token est requis." });
            }

            var utilisateur = await _utilisateurRepository.GetUserByRefreshToken(refreshToken);

            if (utilisateur == null || utilisateur.RefreshTokenExpiry < DateTime.UtcNow)
            {
                Console.WriteLine("⚠️ Tentative de rafraîchissement échouée : Token invalide ou expiré.");
                return Unauthorized(new { message = "Refresh Token invalide ou expiré." });
            }

            var newAccessToken = GenerateAccessToken(utilisateur);
            var newRefreshToken = GenerateRefreshToken();

            utilisateur.RefreshToken = newRefreshToken;
            utilisateur.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _utilisateurRepository.UpdateAsync(utilisateur);

            // ✅ Mise à jour des Cookies
            SetSecureCookies(newAccessToken, newRefreshToken);

            Console.WriteLine($"✅ Token rafraîchi pour : {utilisateur.Email}");
            return Ok(new { AccessToken = newAccessToken, RefreshToken = newRefreshToken });
        }

        // ✅ Endpoint de déconnexion sécurisé
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            if (!Request.Cookies.TryGetValue("RefreshToken", out var refreshToken))
            {
                return BadRequest(new { message = "Aucun Refresh Token trouvé." });
            }

            var utilisateur = await _utilisateurRepository.GetUserByRefreshToken(refreshToken);
            if (utilisateur != null)
            {
                utilisateur.RefreshToken = string.Empty; // ✅ Remplace `null`
                utilisateur.RefreshTokenExpiry = DateTime.UtcNow;
                await _utilisateurRepository.UpdateAsync(utilisateur);
            }

            // ✅ Suppression des Cookies
            Response.Cookies.Delete("AccessToken");
            Response.Cookies.Delete("RefreshToken");

            return Ok(new { message = "Déconnexion réussie !" });
        }

        private void SetSecureCookies(string accessToken, string refreshToken)
        {
            var accessTokenOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(15)
            };

            var refreshTokenOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            Response.Cookies.Append("AccessToken", accessToken, accessTokenOptions);
            Response.Cookies.Append("RefreshToken", refreshToken, refreshTokenOptions);
        }

        private string GenerateAccessToken(Utilisateur utilisateur)
        {
            var secret = _configuration["Jwt:Secret"]
                ?? throw new InvalidOperationException("Le secret JWT n'est pas configuré."); // ✅ Correction CS8604

            var key = Encoding.ASCII.GetBytes(secret);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, utilisateur.Id.ToString()),
                    new Claim(ClaimTypes.Email, utilisateur.Email)
                }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes);
        }

        private string SanitizeInput(string input)
        {
            return input.Replace("'", "").Replace("\"", "").Replace(";", "").Replace("--", "").Trim();
        }

        private bool VerifyPassword(string enteredPassword, string storedHashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(enteredPassword, storedHashedPassword);
        }
    }
}
