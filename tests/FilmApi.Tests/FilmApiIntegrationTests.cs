using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FilmApi.Models;
using Xunit;

namespace FilmApi.Tests;

/// <summary>
/// Tests d'intégration : HTTP → API → Service → Repository → MongoDB.
/// Consigne TD : créer MongoFixture et adapter la WebApplicationFactory ;
/// écrire au moins 2 tests (ex. POST /films → 201, GET /films/{id} après POST → 200).
/// Ce squelette fournit la structure ; les tests sont à compléter / faire passer par l'étudiant.
/// </summary>
public sealed class FilmApiIntegrationTests : IClassFixture<MongoFixture>, IAsyncLifetime, IDisposable
{
    private readonly MongoFixture _mongo;
    private readonly FilmApiAppFactory _factory;
    private readonly HttpClient _client;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public FilmApiIntegrationTests(MongoFixture mongo)
    {
        _mongo = mongo;
        _factory = new FilmApiAppFactory(mongo);
        _client = _factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        await _mongo.InitializeAsync();
        await _mongo.ClearFilmsAsync();
    }

    public void Dispose() => _factory.Dispose();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task POST_films_Returns_201_And_Film()
    {
        // Arrange
        var director = new Director { Id = "d1", Nom = "Dupont", Prenom = "Jean", Nationalite = "FR" };
        var request = new CreateFilmRequest(
            Titre: "Mon Film",
            Resume: "Résumé.",
            Annee: 2024,
            DureeMinutes: 90,
            DateSortie: null,
            Realisateur: director,
            Genres: new List<Genre> { new() { Id = "g1", Libelle = "Drame" } },
            Acteurs: new List<Actor>(),
            PaysProduction: new Country { Code = "FR", Nom = "France" }
        );

        // Act
        var response = await _client.PostAsJsonAsync("/films", request, JsonOptions);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var film = await response.Content.ReadFromJsonAsync<Film>(JsonOptions);
        Assert.NotNull(film);
        Assert.False(string.IsNullOrEmpty(film.Id));
        Assert.Equal("Mon Film", film.Titre);
        Assert.Equal(2024, film.Annee);
    }

    [Fact]
    public async Task GET_films_id_Returns_200_After_Post()
    {
        // Arrange
        var director = new Director { Id = "d2", Nom = "Martin", Prenom = "Marie", Nationalite = "FR" };
        var request = new CreateFilmRequest(
            "Film pour GET",
            "Résumé GET",
            2023,
            100,
            null,
            director,
            new List<Genre> { new() { Id = "g2", Libelle = "Comédie" } },
            new List<Actor>(),
            null
        );
        var postResponse = await _client.PostAsJsonAsync("/films", request, JsonOptions);
        postResponse.EnsureSuccessStatusCode();
        var created = await postResponse.Content.ReadFromJsonAsync<Film>(JsonOptions);
        Assert.NotNull(created);

        // Act
        var response = await _client.GetAsync($"/films/{created.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var film = await response.Content.ReadFromJsonAsync<Film>(JsonOptions);
        Assert.NotNull(film);
        Assert.Equal(created.Id, film.Id);
        Assert.Equal("Film pour GET", film.Titre);
        Assert.Equal("Martin", film.Realisateur.Nom);
    }
}
