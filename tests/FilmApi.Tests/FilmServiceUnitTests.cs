using FilmApi.Models;
using FilmApi.Repositories;
using FilmApi.Services;
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
        var substituteRepo = Substitute.For<IFilmRepository>();
        var director = new DirectorBuilder { Id = "d1", LastName = "Villeneuve", FirstName = "Denis", Nationality = "CA", BirthDate = new DateTime(1967, 10, 3) };
        var genre = new GenreBuilder { Id = "g1", Name = "Science-Fiction" };
        var expectedFilm = new FilmBuilder
        {
            Id = "film1",
            Title = "Dune",
            Summary = "Un jeune duc...",
            Year = 2021,
            DurationMinutes = 155,
            ReleaseDate = new DateTime(2021, 9, 15),
            DirectorBuilder = director,
            Genres = new List<GenreBuilder> { genre },
            Actors = new List<ActorBuilder>(),
            ProductionCountry = new CountryBuilder { Code = "US", Name = "États-Unis" }
        };
        substituteRepo
            .AddAsync(Arg.Any<FilmBuilder>())
            .Returns(expectedFilm);

        var service = new FilmService(substituteRepo);

        var request = new CreateFilmRequest(
            Title: "Dune",
            Summary: "Un jeune duc...",
            Year: 2021,
            DurationMinutes: 155,
            ReleaseDate: new DateTime(2021, 9, 15),
            Director: director,
            Genres: new List<GenreBuilder> { genre },
            Actors: new List<ActorBuilder>(),
            ProductionCountry: new CountryBuilder { Code = "US", Name = "États-Unis" }
        );
        var result = await service.CreateAsync(request);

        Assert.Equal("film1", result.Id);
        Assert.Equal("Dune", result.Title);
        Assert.Equal(2021, result.Year);
        await substituteRepo
            .Received(1)
            .AddAsync(Arg.Is<FilmBuilder>(f => f.Title == "Dune"));
    }

    [Fact]
    public async Task GetByIdAsync_Returns_Film_When_Exists()
    {
        var substituteRepo = Substitute.For<IFilmRepository>();
        var director = new DirectorBuilder { Id = "d2", LastName = "Nolan", FirstName = "Christopher", Nationality = "GB" };
        var film = new FilmBuilder { Id = "f2", Title = "Inception", Year = 2010, DirectorBuilder = director, Genres = new List<GenreBuilder>(), Actors = new List<ActorBuilder>() };
        substituteRepo.GetByIdAsync("f2").Returns(film);

        var service = new FilmService(substituteRepo);
        var result = await service.GetByIdAsync("f2");

        Assert.NotNull(result);
        Assert.Equal("Inception", result.Title);
        Assert.Equal("Nolan", result.Director.LastName);
    }

    [Fact]
    public async Task DeleteAsync_Returns_True_When_Repository_Deletes()
    {
        var substituteRepo = Substitute.For<IFilmRepository>();
        substituteRepo.DeleteByIdAsync("f1").Returns(true);
        var service = new FilmService(substituteRepo);

        var result = await service.DeleteAsync("f1");

        Assert.True(result);
        await substituteRepo.Received(1).DeleteByIdAsync("f1");
    }

    [Fact]
    public async Task DeleteAsync_Returns_False_When_Not_Found()
    {
        var substituteRepo = Substitute.For<IFilmRepository>();
        substituteRepo.DeleteByIdAsync("missing").Returns(false);
        var service = new FilmService(substituteRepo);

        var result = await service.DeleteAsync("missing");

        Assert.False(result);
    }
}
