# ELearningPlatform

> Plateforme e-learning développée en **C# / ASP.NET Core .NET 8**, avec API, client web, accès SQL Server et mécanismes de sécurité applicative.

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![C%23](https://img.shields.io/badge/C%23-ASP.NET_Core-239120?logo=csharp)](https://learn.microsoft.com/aspnet/core/)
[![SQL Server](https://img.shields.io/badge/SQL_Server-Data-CC2927?logo=microsoftsqlserver)](https://www.microsoft.com/sql-server)
[![Security](https://img.shields.io/badge/Security-JWT%20%7C%20BCrypt%20%7C%20Rate%20Limiting-informational)](#sécurité)

## À propos du projet

**ELearningPlatform** est un projet de formation conçu pour mettre en pratique plusieurs dimensions d'un projet applicatif complet : architecture, développement web, gestion des données, authentification, sécurité et documentation.

Le dépôt est conservé comme **projet démonstratif** de mon parcours technique. Il illustre notamment ma capacité à relier développement, infrastructure, base de données et sécurité applicative.

## Objectifs techniques

- Concevoir une application web structurée en plusieurs couches.
- Développer une API avec ASP.NET Core.
- Séparer l'interface, l'API et l'accès aux données.
- Mettre en œuvre une authentification sécurisée.
- Manipuler SQL Server depuis une application .NET.
- Documenter et tester les endpoints avec Swagger / OpenAPI.
- Intégrer des contrôles de sécurité dès la conception.
- Produire une documentation technique et un rapport de sécurité.

## Architecture du dépôt

```text
ELearningPlatform/
├── ELearningPlatform.API/       # Contrôleurs et endpoints API
├── ELearningPlatform.Client/    # Services et partie cliente
├── ELearningPlatform.DAL/       # Accès aux données et repositories
├── Program.cs                   # Configuration et pipeline ASP.NET Core
├── ELearningPlatform.csproj     # Projet .NET 8
└── ELearningPlatform.sln        # Solution Visual Studio
```

L'API contient notamment des contrôleurs dédiés à :

- l'authentification ;
- la gestion des utilisateurs ;
- la gestion des cours ;
- l'accès aux fonctions principales de l'application.

## Technologies utilisées

### Backend

- C#
- .NET 8
- ASP.NET Core
- API REST
- Dependency Injection

### Frontend

- Blazor / composants ASP.NET Core
- Client WebAssembly

### Données

- Microsoft SQL Server
- Microsoft.Data.SqlClient
- Repository pattern

### Sécurité

- JWT Bearer Authentication
- Cookie Authentication
- Cookies `HttpOnly`, `Secure` et `SameSite=Strict`
- BCrypt pour le hachage des mots de passe
- Rate Limiting
- HTTPS / HSTS
- gestion de l'authentification et des autorisations

### API & documentation

- Swagger
- OpenAPI
- documentation des endpoints sécurisés par Bearer token

## Sécurité

La sécurité fait partie intégrante du projet et ne constitue pas uniquement une étape ajoutée en fin de développement.

Le projet met notamment en œuvre :

```text
Utilisateur
    │
    ▼
Authentification
    │
    ├── Cookie sécurisé
    │
    └── JWT Bearer Token
            │
            ▼
       API ASP.NET Core
            │
            ├── Autorisation
            ├── Rate limiting
            └── Accès aux données
                    │
                    ▼
                SQL Server
```

Plusieurs mécanismes sont présents dans le code :

- génération et validation de jetons JWT ;
- cookies d'authentification sécurisés ;
- hachage des mots de passe ;
- limitation du nombre de requêtes ;
- redirection HTTPS ;
- HSTS hors environnement de développement ;
- séparation des responsabilités entre les différentes couches.

> Ce dépôt est un projet de formation. Certaines configurations restent volontairement adaptées à un environnement de laboratoire et doivent être durcies avant tout déploiement en production.

## Ce que ce projet démontre

Ce projet me permet de présenter concrètement plusieurs compétences :

| Domaine | Mise en pratique |
|---|---|
| **Architecture applicative** | Séparation API / client / accès aux données |
| **Développement .NET** | C#, ASP.NET Core, .NET 8 |
| **API** | Contrôleurs REST, Swagger / OpenAPI |
| **Base de données** | SQL Server, accès aux données, repositories |
| **Authentification** | JWT, cookies, gestion d'état |
| **Sécurité applicative** | BCrypt, HTTPS, HSTS, rate limiting |
| **Documentation** | Rapport technique et rapport de sécurité |

## Approche de travail

La méthode suivie sur ce projet peut être résumée ainsi :

```text
Besoin
  ↓
Architecture
  ↓
Développement
  ↓
Tests
  ↓
Analyse des erreurs
  ↓
Corrections
  ↓
Analyse de sécurité
  ↓
Documentation
```

L'objectif n'est pas uniquement d'obtenir une application fonctionnelle, mais également de comprendre **pourquoi elle fonctionne, comment elle peut échouer et comment la sécuriser**.

## Documentation du projet

Les documents produits dans le cadre du travail sont conservés avec le dépôt :

- [Rapport de sécurité ELearningPlatform](https://github.com/user-attachments/files/19025862/Rapport.de.securite.ELearningPlatform.docx)
- [Projet de fin de formation](https://github.com/user-attachments/files/19025860/Projet.de.fin.de.formation.docx)
- [Page de garde du projet](https://github.com/user-attachments/files/19025861/Projet.de.fin.de.formation-pageGarde.docx)

## Limites et pistes d'amélioration

Le repository reflète un projet réalisé dans un contexte d'apprentissage. Les axes d'amélioration identifiés comprennent notamment :

- renforcer la gestion des secrets et de la configuration ;
- restreindre plus finement la politique CORS ;
- augmenter la couverture de tests automatisés ;
- enrichir la journalisation et la supervision ;
- renforcer la gestion centralisée des erreurs ;
- documenter plus précisément les rôles et autorisations ;
- ajouter une chaîne CI/CD et des contrôles de sécurité automatisés ;
- compléter les scénarios de tests de sécurité.

## Positionnement dans mon portfolio

ELearningPlatform complète mes travaux orientés :

- **Infrastructure & systèmes Windows**
- **Cybersécurité / SOC**
- **DFIR & analyse de logs**
- **PowerShell & automatisation**
- **IA appliquée à l'IT et à la cybersécurité**

Il démontre plus particulièrement mon expérience en **développement sécurisé et architecture applicative**.

---

### Fabrice Hacardiaux

Professionnel IT expérimenté — Infrastructure Windows | Cybersécurité / SOC | DFIR

[LinkedIn](https://www.linkedin.com/in/fabricehacardiaux)
