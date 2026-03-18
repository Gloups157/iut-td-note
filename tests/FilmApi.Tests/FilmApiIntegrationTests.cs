using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FilmApi.Models;
using FilmApi.Tests.Builders;
using Xunit;

namespace FilmApi.Tests;

/// <summary>
/// Tests d'intégration : HTTP → API → Service → Repository → MongoDB.
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
        var request = new FilmBuilder()
            .WithTitle("Mon Film")
            .WithSummary("Résumé.")
            .WithYear(2024)
            .WithDurationMinutes(90)
            .WithDirector(new DirectorBuilder().WithLastName("Dupont").WithFirstName("Jean").WithNationality("FR"))
            .ToCreateRequest();

        // Act
        var response = await _client.PostAsJsonAsync("/films", request, JsonOptions);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var film = await response.Content.ReadFromJsonAsync<Film>(JsonOptions);
        Assert.NotNull(film);
        Assert.False(string.IsNullOrEmpty(film.Id));
        Assert.Equal("Mon Film", film.Title);
        Assert.Equal(2024, film.Year);
    }

    [Fact]
    public async Task GET_films_id_Returns_200_After_Post()
    {
        // Arrange
        var request = new FilmBuilder()
            .WithTitle("Film pour GET")
            .WithSummary("Résumé GET")
            .WithYear(2023)
            .WithDurationMinutes(100)
            .WithDirector(new DirectorBuilder().WithLastName("Martin").WithFirstName("Marie").WithNationality("FR"))
            .ToCreateRequest();
            
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
        Assert.Equal("Film pour GET", film.Title);
        Assert.Equal("Martin", film.Director.LastName);
    }

    [Fact]
    public async Task GET_films_FilterByReleaseYear_ReturnsOnlyMatchingFilms()
    {
        // Arrange
        var film2020a = new FilmBuilder()
            .WithTitle("Film 2020 A")
            .WithReleaseDate(new DateTime(2020, 3, 10))
            .ToCreateRequest();

        var film2020b = new FilmBuilder()
            .WithTitle("Film 2020 B")
            .WithReleaseDate(new DateTime(2020, 11, 5))
            .ToCreateRequest();

        var film2023 = new FilmBuilder()
            .WithTitle("Film 2023")
            .WithReleaseDate(new DateTime(2023, 7, 1))
            .ToCreateRequest();

        await _client.PostAsJsonAsync("/films", film2020a, JsonOptions);
        await _client.PostAsJsonAsync("/films", film2020b, JsonOptions);
        await _client.PostAsJsonAsync("/films", film2023, JsonOptions);

        // Act
        var response = await _client.GetAsync("/films?releaseYear=2020");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<Film>>(JsonOptions);
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.All(result.Items, f =>
            Assert.InRange(f.ReleaseDate!.Value, new DateTime(2020, 1, 1), new DateTime(2020, 12, 31)));
    }

    [Fact]
    public async Task DELETE_films_id_FilmNoLongerReturnedInGetFilms()
    {
        // Arrange
        var request = new FilmBuilder()
            .WithTitle("Film à supprimer")
            .WithReleaseDate(new DateTime(2021, 5, 20))
            .ToCreateRequest();

        var postResponse = await _client.PostAsJsonAsync("/films", request, JsonOptions);
        postResponse.EnsureSuccessStatusCode();
        var created = await postResponse.Content.ReadFromJsonAsync<Film>(JsonOptions);
        Assert.NotNull(created);

        // Act
        var deleteResponse = await _client.DeleteAsync($"/films/{created.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        var getResponse = await _client.GetAsync("/films");
        var result = await getResponse.Content.ReadFromJsonAsync<PagedResult<Film>>(JsonOptions);
        Assert.NotNull(result);
        Assert.DoesNotContain(result.Items, f => f.Id == created.Id);
    }
}
