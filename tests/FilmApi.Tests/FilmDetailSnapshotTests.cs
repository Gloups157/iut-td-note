using FilmApi.Models;
using FilmApi.Services;
using FilmApi.Repositories;
using NSubstitute;
using Xunit;
// Verify (VerifyXunit) : à utiliser pour remplacer les Assert par un snapshot (consigne TD §4)

namespace FilmApi.Tests;

/// <summary>
/// État initial du squelette : ce test vérifie un DTO complexe (Film avec Réalisateur, Acteurs, Genres)
/// via une longue série d'Assert.Equal. Consigne TD : refactoriser avec un snapshot Verify
/// (ex. await Verify(filmDetailDto)) et gérer les champs instables (Id, DateTime) avec les options Verify.
/// </summary>
public class FilmDetailSnapshotTests
{
    [Fact]
    public async Task GetById_Returns_Complex_Film_Structure()
    {
        var substituteRepo = Substitute.For<IFilmRepository>();
        var director = new Director
        {
            Id = "dir-1",
            Nom = "Villeneuve",
            Prenom = "Denis",
            Nationalite = "CA",
            DateNaissance = new DateTime(1967, 10, 3)
        };
        var actors = new List<Actor>
        {
            new() { Id = "a1", Nom = "Chalamet", Prenom = "Timothée", Role = "Paul Atréides" },
            new() { Id = "a2", Nom = "Zendaya", Prenom = "", Role = "Chani" }
        };
        var genres = new List<Genre>
        {
            new() { Id = "g1", Libelle = "Science-Fiction" },
            new() { Id = "g2", Libelle = "Aventure" }
        };
        var film = new Film
        {
            Id = "film-abc-123",
            Titre = "Dune",
            Resume = "Sur la planète Arrakis...",
            Annee = 2021,
            DureeMinutes = 155,
            DateSortie = new DateTime(2021, 9, 15),
            Realisateur = director,
            Acteurs = actors,
            Genres = genres,
            PaysProduction = new Country { Code = "US", Nom = "États-Unis" }
        };
        substituteRepo.GetByIdAsync("film-abc-123", Arg.Any<CancellationToken>()).Returns(film);

        var service = new FilmService(substituteRepo);
        var result = await service.GetByIdAsync("film-abc-123");

        Assert.NotNull(result);
        Assert.Equal("film-abc-123", result!.Id);
        Assert.Equal("Dune", result.Titre);
        Assert.Equal("Sur la planète Arrakis...", result.Resume);
        Assert.Equal(2021, result.Annee);
        Assert.Equal(155, result.DureeMinutes);
        Assert.Equal(new DateTime(2021, 9, 15), result.DateSortie);
        Assert.NotNull(result.Realisateur);
        Assert.Equal("dir-1", result.Realisateur.Id);
        Assert.Equal("Villeneuve", result.Realisateur.Nom);
        Assert.Equal("Denis", result.Realisateur.Prenom);
        Assert.Equal("CA", result.Realisateur.Nationalite);
        Assert.Equal(2, result.Acteurs.Count);
        Assert.Equal("Chalamet", result.Acteurs[0].Nom);
        Assert.Equal("Paul Atréides", result.Acteurs[0].Role);
        Assert.Equal(2, result.Genres.Count);
        Assert.Equal("Science-Fiction", result.Genres[0].Libelle);
        Assert.NotNull(result.PaysProduction);
        Assert.Equal("US", result.PaysProduction.Code);
        Assert.Equal("États-Unis", result.PaysProduction.Nom);
    }
}
