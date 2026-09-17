# Revue de sécurité

Cette revue décrit l'état actuel de la sécurité de `ELearningPlatform` à partir du code présent dans le dépôt.

Le projet est un **projet de formation**. Il contient plusieurs mécanismes de sécurité intéressants, mais il ne doit pas être considéré comme prêt pour une mise en production sans durcissement supplémentaire.

L'objectif de ce document n'est pas de présenter une architecture parfaite : il est de montrer **ce qui est déjà protégé, ce qui reste fragile et comment l'améliorer**.

---

## 1. Synthèse

| Domaine | État actuel | Priorité |
|---|---|---:|
| Gestion des secrets dans GitHub | Améliorée / configuration publique neutralisée | Haute |
| Hachage des mots de passe | BCrypt utilisé pour la vérification | Haute |
| JWT | Signature HMAC SHA-256, validation partielle | Haute |
| Cookies | `HttpOnly`, `Secure`, `SameSite=Strict` | Haute |
| Refresh tokens | Génération forte, stockage et exposition à améliorer | Haute |
| Autorisation des endpoints | Contrôles insuffisants sur certains endpoints | Critique |
| Données utilisateur retournées | Risque d'exposition de données sensibles | Critique |
| SQL | Requêtes paramétrées observées | Haute |
| CORS | Politique actuellement trop permissive | Haute |
| Rate limiting | Limiteur global présent | Moyenne |
| HTTPS / HSTS | Présents | Haute |
| Swagger | Limité à l'environnement de développement | Bonne pratique |
| Journalisation | Présente mais à structurer et minimiser | Moyenne |

---

## 2. Gestion des secrets

### Protection actuelle

Le dépôt public ne conserve plus la configuration locale réelle.

Les éléments suivants sont ignorés par Git :

```text
.env
.env.*
secrets.json
appsettings.Development.json
appsettings.Local.json
appsettings.*.local.json
```

Le fichier public :

```text
appsettings.Development.example.json
```

ne contient que des valeurs d'exemple.

### Point d'attention

Une ancienne clé JWT a existé dans l'historique Git avant le nettoyage du dépôt.

Elle doit donc être considérée comme **définitivement exposée** et ne doit plus jamais être réutilisée.

### Amélioration recommandée

Pour une utilisation réelle :

- variables d'environnement ;
- Secret Manager .NET en développement ;
- coffre de secrets en production ;
- rotation des secrets ;
- secret JWT différent par environnement.

---

## 3. Mots de passe

`AuthController` vérifie les mots de passe avec :

```text
BCrypt.Net.BCrypt.Verify()
```

C'est un bon choix pour la vérification d'un mot de passe haché.

### Limite importante

La couche repository manipule directement la propriété :

```text
MotDePasseHache
```

La sécurité dépend donc du fait que cette valeur soit effectivement hachée **avant** son insertion en base.

Le repository `AddAsync()` ne réalise pas lui-même le hachage : il enregistre la valeur reçue.

### Recommandation

Centraliser le hachage avant toute création ou modification d'utilisateur et empêcher l'écriture directe d'un mot de passe non haché.

---

## 4. Exposition des données utilisateur — priorité critique

`UtilisateurController` propose actuellement des opérations CRUD sans attribut `[Authorize]` visible sur le contrôleur ou ses méthodes.

Cela signifie que l'autorisation de ces endpoints doit être revue en priorité.

Plus important encore, le repository charge des propriétés sensibles telles que :

```text
MotDePasseHache
RefreshToken
RefreshTokenExpiry
```

et les objets `Utilisateur` sont ensuite retournés par certains endpoints.

Même un **hash de mot de passe ne doit pas être exposé à un client**.

Un refresh token ne doit jamais être retourné dans un endpoint générique de consultation d'utilisateur.

### Correction recommandée

Ne jamais exposer directement l'entité de base de données.

Utiliser des DTO dédiés, par exemple :

```text
UtilisateurPublicDto
├── Id
├── Nom
├── UserName
├── Surnom
├── Email
└── Role
```

Les propriétés suivantes doivent rester exclusivement côté serveur :

```text
MotDePasseHache
RefreshToken
RefreshTokenExpiry
```

Ajouter ensuite des règles d'autorisation selon les besoins : utilisateur authentifié, administrateur, propriétaire de la ressource, etc.

---

## 5. JWT

### Protection actuelle

Le JWT est signé avec :

```text
HMAC SHA-256
```

La clé de signature provient de :

```text
Jwt:Secret
```

La durée de vie de l'access token est actuellement d'environ 15 minutes.

Le `ClockSkew` est réglé à zéro.

### Limites actuelles

Dans `Program.cs` :

```text
ValidateIssuer = false
ValidateAudience = false
```

La signature est donc contrôlée, mais l'émetteur et le destinataire du token ne sont pas validés.

### Amélioration recommandée

Configurer et valider explicitement :

```text
Issuer
Audience
SigningKey
Lifetime
```

avec des valeurs différentes selon l'environnement.

---

## 6. Refresh tokens

### Points positifs

Le refresh token est généré avec un générateur cryptographiquement sûr :

```text
RandomNumberGenerator
```

La taille utilisée est de 64 octets aléatoires avant encodage Base64.

Une date d'expiration est enregistrée.

Lors d'un rafraîchissement, un nouveau refresh token est généré.

### Risques actuels

Le refresh token est stocké directement en base de données.

Il est également retourné dans le corps JSON des réponses `login` et `refresh`, alors qu'il est déjà déposé dans un cookie sécurisé.

Cela augmente inutilement le nombre d'endroits où le token peut apparaître.

### Améliorations recommandées

- ne pas retourner le refresh token dans le JSON si l'architecture repose sur un cookie `HttpOnly` ;
- stocker idéalement un **hash du refresh token** en base ;
- prévoir révocation, rotation et détection de réutilisation ;
- documenter précisément le cycle de vie du token.

---

## 7. Cookies

Les cookies créés dans `AuthController` utilisent :

```text
HttpOnly = true
Secure = true
SameSite = Strict
```

Ces réglages réduisent notamment l'exposition du token au JavaScript côté navigateur et imposent HTTPS pour leur transmission.

### Point à examiner

Dès qu'une authentification repose sur des cookies, les endpoints modifiant l'état doivent aussi être examinés sous l'angle **CSRF**.

`SameSite=Strict` apporte une protection importante, mais une stratégie explicite doit être définie avant une mise en production.

---

## 8. SQL Server

### Point positif

Les requêtes observées dans `UtilisateurRepository` utilisent des paramètres :

```text
@Id
@Email
@RefreshToken
...
```

C'est la bonne approche pour éviter la concaténation directe de données utilisateur dans les requêtes SQL.

### Attention au faux sentiment de sécurité

`AuthController` contient également une méthode `SanitizeInput()` qui supprime certains caractères.

Cette méthode ne doit **pas** être considérée comme une protection générale contre l'injection SQL.

La véritable protection doit rester :

```text
requêtes paramétrées
+
validation des données
+
contrôle des types
```

---

## 9. Droits SQL

L'application crée actuellement les tables au démarrage si elles n'existent pas.

Cela implique que le compte SQL utilisé possède des permissions suffisamment élevées pour exécuter du DDL.

C'est acceptable dans un laboratoire, mais moins souhaitable en production.

### Amélioration recommandée

Séparer :

```text
Compte de déploiement / migration
        ↓
création et modification du schéma

Compte de l'application
        ↓
SELECT / INSERT / UPDATE / DELETE nécessaires uniquement
```

Principe : **moindre privilège**.

---

## 10. CORS — à durcir

La politique actuelle est :

```text
AllowAnyOrigin
AllowAnyMethod
AllowAnyHeader
```

Elle facilite le développement mais est trop permissive pour une application exposée.

### Production

Définir explicitement les origines autorisées, par exemple :

```text
https://application.exemple.tld
```

et limiter méthodes et headers aux besoins réels.

---

## 11. Rate limiting

Le projet configure un `PartitionedRateLimiter` global.

C'est une bonne base contre certains abus et rafales de requêtes.

### Améliorations possibles

Créer des politiques différentes pour :

```text
/auth/login      → limite stricte
/auth/refresh    → limite stricte
API générale     → limite standard
administration   → règles spécifiques
```

Le rate limiting ne remplace toutefois ni l'authentification ni l'autorisation.

---

## 12. HTTPS et HSTS

Le pipeline contient :

```text
UseHttpsRedirection()
```

et active HSTS hors environnement de développement.

C'est une bonne base.

Pour un déploiement réel, vérifier également :

- certificat valide ;
- versions TLS acceptées ;
- reverse proxy éventuel ;
- headers de sécurité ;
- configuration HSTS adaptée au domaine.

---

## 13. Swagger

Swagger et Swagger UI ne sont activés que lorsque :

```text
app.Environment.IsDevelopment()
```

C'est une bonne séparation entre laboratoire et production.

Une exposition Swagger en production devrait être décidée explicitement et protégée si nécessaire.

---

## 14. Journalisation

Le projet écrit actuellement plusieurs informations dans la console, notamment lors :

- du démarrage ;
- de la connexion SQL ;
- de l'authentification ;
- des échecs de connexion ;
- du rafraîchissement de token.

Certaines traces incluent des adresses e-mail.

### Amélioration recommandée

Passer à une journalisation structurée avec niveaux :

```text
Information
Warning
Error
Critical
```

et éviter d'enregistrer :

- mots de passe ;
- tokens ;
- secrets ;
- données personnelles inutiles.

Cette journalisation structurée pourrait ensuite alimenter un SIEM et faire le lien avec les travaux SOC du portfolio.

---

## 15. Ordre de durcissement proposé

### Priorité 1 — exposition directe

1. protéger `UtilisateurController` avec une politique d'autorisation adaptée ;
2. créer des DTO ne contenant jamais `MotDePasseHache` ou `RefreshToken` ;
3. vérifier le hachage obligatoire à la création et à la modification des utilisateurs.

### Priorité 2 — authentification

4. activer `ValidateIssuer` et `ValidateAudience` ;
5. revoir le stockage et le retour des refresh tokens ;
6. formaliser le mécanisme de révocation.

### Priorité 3 — surface Web

7. remplacer `AllowAll` par une politique CORS restrictive ;
8. examiner la protection CSRF ;
9. créer des politiques de rate limiting spécifiques.

### Priorité 4 — exploitation

10. appliquer le moindre privilège SQL ;
11. structurer les logs ;
12. ajouter tests de sécurité et CI/CD ;
13. ajouter analyse de dépendances et analyse statique.

---

## 16. Démarche sécurité du projet

La progression recherchée est :

```text
Comprendre l'application
        ↓
Identifier les données sensibles
        ↓
Identifier les frontières de confiance
        ↓
Observer l'authentification et les flux
        ↓
Identifier les faiblesses
        ↓
Prioriser les risques
        ↓
Corriger
        ↓
Tester
        ↓
Journaliser
        ↓
Surveiller
```

Cette revue fait volontairement apparaître les limites du projet. Dans un portfolio cybersécurité, reconnaître une faiblesse, expliquer son risque et proposer une correction est plus démonstratif que prétendre qu'une application de laboratoire est parfaitement sécurisée.
