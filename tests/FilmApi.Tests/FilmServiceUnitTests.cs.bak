using FilmApi.Models;
using FilmApi.Repositories;
using FilmApi.Services;
using FilmApi.Tests.Builders;
using NSubstitute;
using Xunit;

namespace FilmApi.Tests;

/// <summary>
/// Tests unitaires du FilmService avec un mock du repository.
/// </summary>
public class FilmServiceUnitTests
{
    [Fact]
    public async Task CreateAsync_Calls_Repository_AddAsync_And_Returns_Film()
    {
        // Arrange
        var substituteRepo = Substitute.For<IFilmRepository>();
        var directorBuilder = new DirectorBuilder()
            .WithId("d1")
            .WithLastName("Villeneuve")
            .WithFirstName("Denis")
            .WithNationality("CA")
            .WithBirthDate(new DateTime(1967, 10, 3));

        var genreBuilder = new GenreBuilder()
            .WithId("g1")
            .WithName("Science-Fiction");

        var expectedFilmBuilder = new FilmBuilder()
            .WithId("film1")
            .WithTitle("Dune")
            .WithSummary("Un jeune duc...")
            .WithYear(2021)
            .WithDurationMinutes(155)
            .WithReleaseDate(new DateTime(2021, 9, 15))
            .WithDirector(directorBuilder)
            .AddGenre(genreBuilder)
            .WithoutActors()
            .WithCountry(new CountryBuilder()
                .WithCode("US")
                .WithName("États-Unis"));

        var expectedFilm = expectedFilmBuilder.Build();
        substituteRepo.AddAsync(Arg.Any<Film>()).Returns(expectedFilm);

        var service = new FilmService(substituteRepo);

        var request = new CreateFilmRequestBuilder()
            .WithTitle("Dune")
            .WithSummary("Un jeune duc...")
            .WithYear(2021)
            .WithDurationMinutes(155)
            .WithReleaseDate(new DateTime(2021, 9, 15))
            .WithDirector(directorBuilder)
            .WithGenres(new List<GenreBuilder>{genreBuilder})
            .WithoutActors()
            .WithProductionCountry(new CountryBuilder().
                WithCode("US").
                WithName("États-Unis"))
            .Build();
        
        // Act
        var result = await service.CreateAsync(request);

        // Assert
        Assert.Equal("film1", result.Id);
        Assert.Equal("Dune", result.Title);
        Assert.Equal(2021, result.Year);
        await substituteRepo.Received(1).AddAsync(Arg.Is<Film>(f => f.Title == "Dune"));
    }

    [Fact]
    public async Task GetByIdAsync_Returns_Film_When_Exists()
    {
        // Arrange
        var substituteRepo = Substitute.For<IFilmRepository>();
        var directorBuilder = new DirectorBuilder()
            .WithId("d2")
            .WithLastName("Nolan")
            .WithFirstName("Christopher")
            .WithNationality("GB");

        var filmBuilder = new FilmBuilder()
            .WithId("f2")
            .WithTitle("Inception")
            .WithYear(2010)
            .WithDirector(directorBuilder)
            .WithoutGenres()
            .WithoutActors();

        var expectedFilm = filmBuilder.Build();
        substituteRepo.GetByIdAsync("f2").Returns(expectedFilm);

        var service = new FilmService(substituteRepo);

        // Act
        var result = await service.GetByIdAsync("f2");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Inception", result.Title);
        Assert.Equal("Nolan", result.Director.LastName);
    }

    [Fact]
    public async Task DeleteAsync_Returns_True_When_Repository_Deletes()
    {
        // Arrange
        var substituteRepo = Substitute.For<IFilmRepository>();
        substituteRepo.DeleteByIdAsync("f1").Returns(true);
        var service = new FilmService(substituteRepo);

        // Act
        var result = await service.DeleteAsync("f1");

        // Assert
        Assert.True(result);
        await substituteRepo.Received(1).DeleteByIdAsync("f1");
    }

    [Fact]
    public async Task DeleteAsync_Returns_False_When_Not_Found()
    {
        // Arrange
        var substituteRepo = Substitute.For<IFilmRepository>();
        substituteRepo.DeleteByIdAsync("missing").Returns(false);
        var service = new FilmService(substituteRepo);

        // Act
        var result = await service.DeleteAsync("missing");

        // Assert
        Assert.False(result);
        await substituteRepo.Received(1).DeleteByIdAsync("missing");
    }
}
