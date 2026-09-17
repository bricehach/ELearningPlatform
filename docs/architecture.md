# Architecture du projet

Ce document présente l'architecture actuelle de `ELearningPlatform` telle qu'elle apparaît dans le dépôt.

Le projet est une application **ASP.NET Core .NET 8** structurée en plusieurs dossiers logiques. Il ne s'agit pas, à ce stade, de microservices séparés : l'ensemble est regroupé dans une même application Web .NET.

## Vue d'ensemble

```text
Utilisateur / navigateur
        ↓
Client / composants Web
        ↓
API ASP.NET Core
        ↓
Services / authentification
        ↓
Repositories DAL
        ↓
Microsoft.Data.SqlClient
        ↓
SQL Server
```

## Organisation du dépôt

```text
ELearningPlatform/
├── ELearningPlatform.API/
│   └── Controllers/
├── ELearningPlatform.Client/
│   ├── Helpers/
│   ├── Pages/
│   ├── Services/
│   └── Shared/
├── ELearningPlatform.DAL/
│   ├── Data/
│   ├── Helpers/
│   ├── Models/
│   └── Repositories/
├── Properties/
├── Program.cs
├── ELearningPlatform.csproj
├── ELearningPlatform.sln
├── appsettings.json
└── appsettings.Development.example.json
```

## 1. Couche API

Le dossier `ELearningPlatform.API/Controllers` expose actuellement plusieurs contrôleurs HTTP :

- `AuthController` ;
- `CoursController` ;
- `IndexController` ;
- `UtilisateurController`.

Les routes sont prises en charge par ASP.NET Core avec `MapControllers()`.

Swagger/OpenAPI est activé en environnement de développement afin de permettre l'exploration et le test des endpoints.

## 2. Partie cliente

Le dossier `ELearningPlatform.Client` contient notamment :

- les pages et composants ;
- les services d'authentification ;
- un `AuthenticationStateProvider` ;
- un `TokenInterceptor` ;
- les modèles partagés utilisés côté client.

Un `HttpClient` nommé `API` est configuré vers :

```text
https://localhost:7103/api/
```

et utilise le `TokenInterceptor`.

## 3. Couche d'accès aux données

Le dossier `ELearningPlatform.DAL` regroupe :

- les modèles métier ;
- les repositories ;
- les helpers SQL ;
- l'initialisation de la base.

L'accès aux données utilise directement `Microsoft.Data.SqlClient`.

Le projet n'utilise pas Entity Framework Core dans son état actuel.

### Repositories

`UtilisateurRepository` encapsule les opérations SQL liées aux utilisateurs :

```text
GetAllAsync
GetByIdAsync
GetByEmailAsync
GetUserByRefreshToken
AddAsync
UpdateAsync
DeleteAsync
```

Les requêtes observées utilisent des paramètres SQL tels que `@Id`, `@Email` et `@RefreshToken`.

## 4. Initialisation SQL Server

Au démarrage, `Program.cs` :

1. récupère `ConnectionStrings:DefaultConnection` ;
2. teste la connexion ;
3. appelle `DatabaseInitializer.InitializeDatabase()`.

`DatabaseInitializer` crée les tables si elles n'existent pas encore :

```text
Utilisateurs
Cours
Progression
Evaluations
Modules
Inscriptions
```

La base SQL Server elle-même doit toutefois déjà exister.

## 5. Authentification

L'application combine actuellement :

- Cookie Authentication ;
- JWT Bearer Authentication ;
- access token ;
- refresh token.

### Flux simplifié de connexion

```text
Utilisateur
    ↓
POST /api/auth/login
    ↓
Recherche utilisateur par email
    ↓
Vérification BCrypt du mot de passe
    ↓
Création d'un JWT
    ↓
Création d'un refresh token aléatoire
    ↓
Enregistrement du refresh token en base
    ↓
Cookies sécurisés + réponse HTTP
```

L'access token expire actuellement après environ 15 minutes et le refresh token après 7 jours.

## 6. Cookies

Les cookies créés par `AuthController` utilisent actuellement :

```text
HttpOnly = true
Secure = true
SameSite = Strict
```

Les cookies concernés sont :

- `AccessToken` ;
- `RefreshToken`.

La configuration générale d'authentification définit également un cookie `ELearningPlatformAuth`.

## 7. JWT

Le JWT est signé avec une clé symétrique et l'algorithme HMAC SHA-256.

Le secret est lu depuis :

```text
Jwt:Secret
```

La configuration publique ne contient volontairement aucun véritable secret.

## 8. Rate limiting

Un limiteur global est configuré avec :

```text
PartitionedRateLimiter<HttpContext, string>
```

La logique de partition est déléguée à `RateLimitHelper`.

Le middleware est activé par :

```text
app.UseRateLimiter();
```

## 9. CORS

Une politique nommée `AllowAll` est actuellement configurée avec :

```text
AllowAnyOrigin
AllowAnyMethod
AllowAnyHeader
```

Cette configuration facilite le laboratoire mais ne doit pas être considérée comme une configuration de production.

## 10. Pipeline HTTP

Le pipeline actuel comprend notamment :

```text
Rate Limiting
    ↓
Authentication
    ↓
Authorization
    ↓
Default / Static Files
    ↓
HTTPS Redirection
    ↓
Routing
    ↓
CORS
    ↓
Controllers
```

En production, HSTS et un gestionnaire d'exception sont activés. En développement, Swagger et la page d'exception développeur sont disponibles.

## 11. Architecture de sécurité simplifiée

```text
                   ┌─────────────────┐
                   │   Utilisateur   │
                   └────────┬────────┘
                            │
                            ▼
                   ┌─────────────────┐
                   │ Client / HTTP   │
                   └────────┬────────┘
                            │
                 HTTPS / cookies / JWT
                            │
                            ▼
                   ┌─────────────────┐
                   │ API ASP.NET Core│
                   └────────┬────────┘
                            │
              Auth / rate limiting / CORS
                            │
                            ▼
                   ┌─────────────────┐
                   │ Repositories DAL│
                   └────────┬────────┘
                            │
                     requêtes SQL
                            │
                            ▼
                   ┌─────────────────┐
                   │   SQL Server    │
                   └─────────────────┘
```

## 12. Ce que cette architecture permet d'étudier

Ce projet est surtout intéressant comme support d'apprentissage pour comprendre :

- la séparation logique entre interface, API et accès aux données ;
- la circulation d'une requête jusqu'à SQL Server ;
- la gestion des identités et des tokens ;
- la surface d'attaque d'une API Web ;
- l'endroit où placer des contrôles de sécurité ;
- les traces qu'une application moderne peut produire pour un SOC ou une investigation DFIR.

Cette architecture sert donc de pont entre **développement applicatif, infrastructure et cybersécurité**.
