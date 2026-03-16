using FilmApi.Models;
using FilmApi.Repositories;
using FilmApi.Services;
using NSubstitute;
using Xunit;

namespace FilmApi.Tests;

/// <summary>
/// Tests unitaires du FilmService avec un mock du repository.
/// État initial du squelette : à refactoriser en AAA (Arrange / Act / Assert)
/// et avec des test data builders (FilmBuilder, DirectorBuilder, etc.) au lieu de construire les objets à la main.
/// </summary>
public class FilmServiceUnitTests
{
    [Fact]
    public async Task CreateAsync_Calls_Repository_AddAsync_And_Returns_Film()
    {
        var substituteRepo = Substitute.For<IFilmRepository>();
        var director = new Director { Id = "d1", Nom = "Villeneuve", Prenom = "Denis", Nationalite = "CA", DateNaissance = new DateTime(1967, 10, 3) };
        var genre = new Genre { Id = "g1", Libelle = "Science-Fiction" };
        var expectedFilm = new Film
        {
            Id = "film1",
            Titre = "Dune",
            Resume = "Un jeune duc...",
            Annee = 2021,
            DureeMinutes = 155,
            DateSortie = new DateTime(2021, 9, 15),
            Realisateur = director,
            Genres = new List<Genre> { genre },
            Acteurs = new List<Actor>(),
            PaysProduction = new Country { Code = "US", Nom = "États-Unis" }
        };
        substituteRepo
            .AddAsync(Arg.Any<Film>(), Arg.Any<CancellationToken>())
            .Returns(expectedFilm);

        var service = new FilmService(substituteRepo);

        var request = new CreateFilmRequest(
            Titre: "Dune",
            Resume: "Un jeune duc...",
            Annee: 2021,
            DureeMinutes: 155,
            DateSortie: new DateTime(2021, 9, 15),
            Realisateur: director,
            Genres: new List<Genre> { genre },
            Acteurs: new List<Actor>(),
            PaysProduction: new Country { Code = "US", Nom = "États-Unis" }
        );
        var result = await service.CreateAsync(request);

        Assert.Equal("film1", result.Id);
        Assert.Equal("Dune", result.Titre);
        Assert.Equal(2021, result.Annee);
        await substituteRepo
            .Received(1)
            .AddAsync(Arg.Is<Film>(f => f.Titre == "Dune"), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetByIdAsync_Returns_Film_When_Exists()
    {
        var substituteRepo = Substitute.For<IFilmRepository>();
        var director = new Director { Id = "d2", Nom = "Nolan", Prenom = "Christopher", Nationalite = "GB" };
        var film = new Film { Id = "f2", Titre = "Inception", Annee = 2010, Realisateur = director, Genres = new List<Genre>(), Acteurs = new List<Actor>() };
        substituteRepo.GetByIdAsync("f2", Arg.Any<CancellationToken>()).Returns(film);

        var service = new FilmService(substituteRepo);
        var result = await service.GetByIdAsync("f2");

        Assert.NotNull(result);
        Assert.Equal("Inception", result.Titre);
        Assert.Equal("Nolan", result.Realisateur.Nom);
    }
}
