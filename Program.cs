using ELearningPlatform.DAL.Data;
using ELearningPlatform.DAL.Helpers;
using ELearningPlatform.API.Helpers;
using ELearningPlatform.DAL.Repositories;
using ELearningPlatform.Client.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.OpenApi.Models;
using ELearningPlatform.ELearningPlatform.Client.Services;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        Console.WriteLine("🔍 Démarrage de l’application et vérification de la connexion SQL...");

        // 📌 Récupération de la chaîne de connexion
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            Console.WriteLine("❌ Erreur : La chaîne de connexion est null ou vide.");
            return;
        }

        // ✅ Vérification de la connexion SQL avant de démarrer l'application
        DatabaseHelper.TestDatabaseConnection(connectionString);

        // ✅ Initialisation de la base de données
        DatabaseInitializer.InitializeDatabase(builder.Configuration);

        // 📌 Enregistrement des services et repositories
        builder.Services.AddSingleton<IUtilisateurRepository>(new UtilisateurRepository(connectionString));
        builder.Services.AddScoped<IAuthService, AuthService>();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        });

        // ✅ Enregistrement du service IHttpContextAccessor
        builder.Services.AddHttpContextAccessor();

        // ✅ Enregistrement de `AuthStateProvider`
        builder.Services.AddScoped<AuthStateProvider>();
        builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<AuthStateProvider>());

        // ✅ Ajout du TokenInterceptor
        builder.Services.AddScoped<TokenInterceptor>();

        // ✅ Configuration `HttpClient` avec `TokenInterceptor`
        builder.Services.AddHttpClient("API", client =>
        {
            client.BaseAddress = new Uri("https://localhost:7103/api/");
        }).AddHttpMessageHandler<TokenInterceptor>();

        // ✅ Configuration de l’authentification `JWT` avec `Cookie Authentication`
        var secretKey = builder.Configuration["Jwt:Secret"];
        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException("Le secret JWT n'est pas configuré.");
        }

        var key = Encoding.UTF8.GetBytes(secretKey);

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
            options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
            options.Cookie.Name = "ELearningPlatformAuth";
            options.ExpireTimeSpan = TimeSpan.FromMinutes(15);
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };
        });

        // ✅ Ajout et configuration de Swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "ELearningPlatform API",
                Version = "v1",
                Description = "Documentation de l'API ELearningPlatform"
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Saisissez 'Bearer {votre_token}' pour accéder aux endpoints sécurisés"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    new string[] {}
                }
            });
        });

        // ✅ Configuration `Rate Limiting`
        builder.Services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context => RateLimitHelper.GetRateLimitPartition(context));
        });

        builder.Services.AddControllers();
        builder.Services.AddHttpClient();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ELearningPlatform API v1");
                c.RoutePrefix = string.Empty;
            });
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseRateLimiter();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseDefaultFiles();
        app.UseStaticFiles();
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseCors("AllowAll");

        app.MapControllers();
        app.Run();
    }
}
