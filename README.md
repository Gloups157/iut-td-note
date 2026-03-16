# TD noté — Tests et performance (3h)

Sujet de **TD noté** : FilmApi (API Films, MongoDB), refacto des tests (AAA, test data builders, snapshot testing avec Verify), tests d’intégration avec Testcontainers.MongoDb, et tests de performance (k6, InfluxDB, Grafana) via Aspire.

---

## Prérequis

- **.NET 10** SDK
- **Docker** (pour Testcontainers en tests, et pour MongoDB / InfluxDB / Grafana via l’AppHost)
- **k6** (optionnel, pour lancer les scripts de charge depuis la ligne de commande)
- IDE (Visual Studio, Rider ou VS Code)

La solution est au format **.slnx** (fichier XML). Ouvrir `FilmApi.slnx` à la racine ou utiliser `dotnet sln` avec ce fichier.

---

## Structure du projet

```
├── FilmApi.slnx
├── AppHost/                    # Aspire : MongoDB, FilmApi, seeds (50k, 500k), InfluxDB, Grafana
├── src/
│   └── FilmApi/                # API (GET/POST /films), modèle Film imbriqué, MongoDB
│       ├── Models/             # Film, Director, Actor, Genre, Country, CreateFilmRequest, PagedResult
│       ├── Repositories/       # IFilmRepository, FilmRepository
│       └── Services/          # IFilmService, FilmService
├── scripts/
│   ├── SeedFilms/              # Application console : seed 50k ou 500k (args)
│   ├── lib/                    # config.js partagé pour k6 (BASE_URL, TOTAL_ITEMS, /films)
│   ├── load-test/              # load-test.js (50k ou 500k selon TOTAL_ITEMS)
│   └── spike-test/             # spike-test.js (50k ou 500k)
├── grafana/
│   └── provisioning/          # Datasource InfluxDB + dashboard k6 (même que iut-performance-testing)
└── tests/
    └── FilmApi.Tests/          # Unitaires (mock) + intégration (MongoFixture, WebApplicationFactory)
        ├── MongoFixture.cs
        ├── FilmApiAppFactory.cs
        ├── FilmServiceUnitTests.cs      # À refactoriser : AAA + builders
        ├── FilmDetailSnapshotTests.cs   # À refactoriser : snapshot Verify
        └── FilmApiIntegrationTests.cs   # Intégration HTTP → MongoDB
```

---

## Objectifs du TD (3h)

1. **Refacto AAA** : tous les tests (unitaires et intégration) avec blocs **Arrange / Act / Assert** clairs.
2. **Test data builders** : introduire **FilmBuilder** (et éventuellement DirectorBuilder, ActorBuilder), refactoriser **au moins 3 tests** pour les utiliser.
3. **Snapshot testing** : refactoriser **au moins un test** (ex. FilmDetailSnapshotTests) avec **Verify** ; gérer Id / DateTime avec les options Verify.
4. **Tests d’intégration MongoDB** : MongoFixture + WebApplicationFactory déjà fournis ; écrire / compléter **au moins 2 tests** (POST 201, GET après POST).
5. **Aspire** : lancer une **seed** (50k ou 500k), puis un **test de perf** (load ou spike, 50k ou 500k) ; consulter les métriques dans **Grafana**.

---

## Workflow — Tests de performance

1. **Lancer l’AppHost** : `dotnet run --project AppHost` (à la racine).
2. **Dans le tableau de bord Aspire** :
   - Démarrer **film-api** (et les conteneurs MongoDB, InfluxDB, Grafana).
   - Lancer **une seed** : **seed-50k** ou **seed-500k** (démarrage explicite). Attendre la fin de l’insertion.
3. **Lancer un test de performance** :
   - **Dossiers** : `scripts/load-test/` et `scripts/spike-test/`.
   - **Deux exécutions par dossier** : 50k (TOTAL_ITEMS=50000) et 500k (TOTAL_ITEMS=500000).
   - Depuis un terminal (avec k6 installé), en utilisant l’URL de **film-api** affichée dans le dashboard :
     ```bash
     k6 run -e BASE_URL=http://localhost:XXXX -e TOTAL_ITEMS=50000 scripts/load-test/load-test.js
     k6 run -e BASE_URL=http://localhost:XXXX -e TOTAL_ITEMS=500000 scripts/load-test/load-test.js
     k6 run -e BASE_URL=http://localhost:XXXX -e TOTAL_ITEMS=50000 scripts/spike-test/spike-test.js
     k6 run -e BASE_URL=http://localhost:XXXX -e TOTAL_ITEMS=500000 scripts/spike-test/spike-test.js
     ```
   - Remplacer `XXXX` par le port de l’API (voir le dashboard Aspire).
4. **Métriques** : k6 peut envoyer les métriques vers InfluxDB (option `-o xk6-influxdb` si configuré) ; **Grafana** (port 3000) est provisionné avec le datasource InfluxDB et le dashboard **k6 Load Testing**.

---

## Consignes par partie (critères de notation)

### 1. Refacto AAA (§2)

- **Objectif** : Chaque test doit avoir des commentaires ou un découpage clair **Arrange** (données, mocks), **Act** (un seul appel métier ou HTTP), **Assert** (uniquement des vérifications).
- **Critères** : lisibilité, séparation nette des trois blocs, pas d’assertion dans l’Act.

### 2. Test Data Builders (§3)

- **Objectif** : Avec le modèle **Film** imbriqué (Réalisateur, Acteurs, Genres, etc.), construire des instances en test via des **builders** (valeurs par défaut, surcharge uniquement de ce qui compte).
- **À faire** : introduire **FilmBuilder** (API fluide : `WithTitle`, `WithYear`, `WithDirector`, `WithActors`, `WithGenres`, etc.), et éventuellement **DirectorBuilder**, **ActorBuilder**. Refactoriser **au moins 3 tests** pour utiliser ces builders.
- **Critères** : API fluide, valeurs par défaut cohérentes, au moins un test avec customisation partielle (ex. seul le titre ou seul le réalisateur change).

### 3. Snapshot testing — Verify (§4)

- **Objectif** : Remplacer les longues séries d’`Assert.Equal` sur un DTO complexe par **un snapshot** Verify.
- **À faire** : refactoriser **au moins un test** (ex. `FilmDetailSnapshotTests`) en utilisant Verify (ex. `await Verify(filmDetailDto)`). Générer le fichier `.verified.*` au premier run. Pour les champs instables (Id, Guid, DateTime), utiliser les paramètres Verify (scoped settings, ignore de membres). **Verify.Xunit** est déjà installé dans le projet de tests ; s’appuyer sur la [documentation Verify](https://github.com/VerifyTests/Verify).
- **Critères** : au moins un test converti en snapshot sur une sortie « Film complexe » ; snapshot stable ; fichier snapshot versionné ou expliqué dans le README.

### 4. Tests d’intégration MongoDB (§6)

- **Objectif** : Les tests d’intégration font tourner l’API avec une base **MongoDB** (Testcontainers.MongoDb).
- **À faire** : **MongoFixture** et **WebApplicationFactory** sont fournis. Écrire / compléter **au moins 2 tests** d’intégration (ex. `POST /films` → 201 + film retourné ; `GET /films/{id}` après un POST → 200 avec les bonnes données).
- **Critères** : conteneur MongoDB partagé (IClassFixture), API lancée avec MongoDB en tests, au moins 2 tests passants (HTTP → Service → MongoDB).

### 5. Aspire — Seeds et perf (§5)

- **Objectif** : Workflow (1) seed 50k ou 500k, (2) test de perf (load ou spike, 50k ou 500k), (3) visualisation Grafana.
- **Critères** : MongoDB, InfluxDB et Grafana démarrés avec l’AppHost ; workflow respecté ; seed-50k et seed-500k fonctionnels ; tests de perf lancés (dossiers load-test / spike-test, exécutions 50k et 500k) ; métriques visibles dans Grafana.

---

## Répartition du temps suggérée (3h)

| Partie | Durée  | Contenu |
|--------|--------|--------|
| 1      | 20 min | Découverte du squelette, lancement Aspire ; lancer une seed (50k ou 500k), puis un test de perf (load ou spike) ; visualisation Grafana |
| 2      | 35 min | Refacto AAA sur les tests existants |
| 3      | 40 min | Test data builders : FilmBuilder (+ éventuellement DirectorBuilder, ActorBuilder), refacto d’au moins 3 tests |
| 4      | 35 min | Snapshot testing : Verify + au moins un test refactoré (objet ou chaîne), gestion des champs instables |
| 5      | 50 min | Tests d’intégration MongoDB : MongoFixture, WebApplicationFactory, au moins 2 tests |

---

## Commandes utiles

```bash
# Restaurer et compiler
dotnet restore
dotnet build

# Lancer l’AppHost (MongoDB, API, seeds, InfluxDB, Grafana)
dotnet run --project AppHost

# Tests
dotnet test tests/FilmApi.Tests
```

---

## Références

- Plan de conception : **PLAN.md** (à la racine).
- Suivi du développement du squelette : **DEVELOPMENT.md** (à la racine).
- Verify : [Verify / GitHub](https://github.com/VerifyTests/Verify).
- iut-integration-testing, iut-test-data-builder, iut-snapshot-testing, iut-performance-testing (projets de référence).
