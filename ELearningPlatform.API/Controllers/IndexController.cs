using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace ELearningPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IndexController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public IndexController(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        // ✅ Endpoint pour récupérer l'heure actuelle
        [HttpGet("time")]
        public IActionResult GetCurrentTime()
        {
            return Ok(new
            {
                Heure = DateTime.Now.ToString("HH:mm:ss"),
                Date = DateTime.Now.ToString("yyyy-MM-dd")
            });
        }

        // ✅ Endpoint pour récupérer la météo
        [HttpGet("weather")]
        public async Task<IActionResult> GetWeather(string city = "Paris")
        {
            try
            {
                string apiKey = _configuration["WeatherAPI:ApiKey"]
                    ?? throw new InvalidOperationException("Clé API manquante dans la configuration."); // ✅ Correction CS8600

                string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}&units=metric";
                var response = await _httpClient.GetStringAsync(url);
                var data = JObject.Parse(response);

                var temp = data["main"]?["temp"]?.ToString() ?? "Donnée indisponible"; // ✅ Correction CS8602
                var conditions = data["weather"]?[0]?["description"]?.ToString() ?? "Donnée indisponible"; // ✅ Correction CS8602

                return Ok(new
                {
                    Ville = city,
                    Température = temp + "°C",
                    Conditions = conditions
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Erreur = "Impossible de récupérer la météo", Détails = ex.Message });
            }
        }
    }
}
