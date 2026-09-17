# Installation et exécution locale

Ce document décrit une installation **locale / laboratoire** de `ELearningPlatform` à partir du dépôt public.

> Le dépôt public ne contient volontairement aucun secret réel ni configuration propre à une machine. Les valeurs locales doivent rester hors de GitHub.

## 1. Prérequis

Prévoir au minimum :

- **.NET 8 SDK** ;
- **SQL Server** ou **SQL Server Express** ;
- Git ;
- un IDE au choix : Visual Studio 2022, VS Code ou équivalent ;
- facultatif : SQL Server Management Studio pour administrer la base.

Le projet cible `net8.0` et utilise notamment ASP.NET Core, Microsoft.Data.SqlClient, JWT, BCrypt et Swagger/OpenAPI.

## 2. Récupérer le projet

```bash
git clone https://github.com/bricehach/ELearningPlatform.git
cd ELearningPlatform
```

Restaurer ensuite les dépendances :

```bash
dotnet restore
```

## 3. Préparer SQL Server

Le code initialise automatiquement les **tables** nécessaires au démarrage, mais il attend qu'une base SQL Server accessible existe déjà.

Créer une base nommée par exemple :

```sql
CREATE DATABASE ELearningPlatform;
```

Les tables créées automatiquement par l'application sont actuellement :

- `Utilisateurs` ;
- `Cours` ;
- `Progression` ;
- `Evaluations` ;
- `Modules` ;
- `Inscriptions`.

## 4. Créer la configuration locale

Le dépôt fournit :

```text
appsettings.Development.example.json
```

Créer localement une copie nommée :

```text
appsettings.Development.json
```

Sous PowerShell :

```powershell
Copy-Item .\appsettings.Development.example.json .\appsettings.Development.json
```

Ce fichier est ignoré par Git et ne doit pas être ajouté au dépôt.

Adapter la chaîne de connexion à l'instance SQL locale. Exemple :

```json
"DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=ELearningPlatform;Integrated Security=True;TrustServerCertificate=True"
```

## 5. Configurer le secret JWT

Ne pas utiliser le placeholder du fichier d'exemple comme véritable clé.

Une possibilité simple pour un laboratoire consiste à définir le secret comme variable d'environnement avant de lancer l'application :

```powershell
$env:Jwt__Secret = "VOTRE_SECRET_LOCAL_LONG_ET_ALEATOIRE"
```

ASP.NET Core traduit `Jwt__Secret` en `Jwt:Secret` dans la configuration.

Le secret doit être suffisamment long, aléatoire et réservé à l'environnement local. Il ne doit jamais être publié dans GitHub, une capture d'écran ou un document partagé.

## 6. Lancer l'application

Depuis la racine du projet :

```bash
dotnet run
```

Au démarrage, l'application :

1. lit la chaîne de connexion ;
2. teste l'accès SQL Server ;
3. tente d'initialiser les tables ;
4. configure les services, l'authentification et le rate limiting ;
5. démarre l'application ASP.NET Core.

En environnement `Development`, Swagger est activé et configuré à la racine de l'application.

La configuration actuelle du projet utilise notamment :

```text
https://localhost:7103
```

Le port réel peut dépendre du profil de lancement utilisé.

## 7. Certificat HTTPS de développement

Si le navigateur refuse le certificat local, vérifier les certificats .NET :

```bash
dotnet dev-certs https --check
```

Sous Windows, pour faire confiance au certificat de développement :

```bash
dotnet dev-certs https --trust
```

## 8. Problèmes courants

### `Le secret JWT n'est pas configuré`

Vérifier que `Jwt:Secret` est fourni par `appsettings.Development.json` ou par la variable d'environnement :

```text
Jwt__Secret
```

### Erreur de connexion SQL

Vérifier :

- que le service SQL Server est démarré ;
- le nom de l'instance (`SQLEXPRESS`, instance nommée, etc.) ;
- l'existence de la base `ELearningPlatform` ;
- les droits du compte Windows si `Integrated Security=True` est utilisé.

### Les tables ne sont pas créées

L'application ne crée pas actuellement la base SQL Server elle-même. Elle crée les tables dans la base indiquée par `DefaultConnection`.

## 9. Configuration publique vs configuration locale

Le principe retenu pour ce dépôt est :

```text
GitHub public
    ├── code source
    ├── appsettings.json neutralisé
    └── appsettings.Development.example.json

Machine locale
    └── appsettings.Development.json
        ├── vraie instance SQL
        └── secrets de développement
```

Les dossiers `bin/` et `obj/`, les fichiers Visual Studio utilisateur et les configurations contenant des secrets sont exclus du dépôt public.

## 10. Contexte du projet

`ELearningPlatform` est un projet de formation et de compréhension technique. Il sert à étudier le fonctionnement d'une application ASP.NET Core, son API, son accès SQL Server et ses mécanismes d'authentification afin de mieux analyser ensuite ses besoins de sécurisation.
