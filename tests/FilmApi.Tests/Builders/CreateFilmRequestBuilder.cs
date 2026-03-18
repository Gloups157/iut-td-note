using System;
using System.Collections.Generic;
using FilmApi.Tests.Builders;

namespace FilmApi.Models;

public class CreateFilmRequestBuilder
{
    // Valeurs par défaut réalistes
    private string _title = "Film par défaut";
    private string _summary = "Résumé générique d'un film générique.";
    private int _year = 2024;
    private int _durationMinutes = 120;
    private DateTime? _releaseDate = DateTime.UtcNow;
    private DirectorBuilder _director = new();
    private List<GenreBuilder> _genres = new(); 
    private List<ActorBuilder> _actors = new();
    private CountryBuilder _productionCountry = new();

    public CreateFilmRequestBuilder WithTitle(string title)
    {
        _title = title ?? string.Empty;
        return this;
    }

    public CreateFilmRequestBuilder WithSummary(string summary)
    {
        _summary = summary ?? string.Empty;
        return this;
    }

    public CreateFilmRequestBuilder WithYear(int year)
    {
        _year = year;
        return this;
    }

    public CreateFilmRequestBuilder WithDurationMinutes(int durationMinutes)
    {
        _durationMinutes = durationMinutes;
        return this;
    }

    public CreateFilmRequestBuilder WithReleaseDate(DateTime? releaseDate)
    {
        _releaseDate = releaseDate;
        return this;
    }

    public CreateFilmRequestBuilder WithDirector(DirectorBuilder director)
    {
        _director = director;
        return this;
    }

    public CreateFilmRequestBuilder WithGenres(List<GenreBuilder> genres)
    {
        _genres = genres;
        return this;
    }

    public CreateFilmRequestBuilder WithActors(List<ActorBuilder> actors)
    {
        _actors = actors;
        return this;
    }

    public CreateFilmRequestBuilder WithProductionCountry(CountryBuilder productionCountry)
    {
        _productionCountry = productionCountry;
        return this;
    }

    public CreateFilmRequestBuilder WithoutGenres()
    {
        _genres.Clear();
        return this;
    }

    public CreateFilmRequestBuilder WithoutActors()
    {
        _actors.Clear();
        return this;
    }

    public CreateFilmRequest Build()
    {
        return new CreateFilmRequest(
            Title: _title,
            Summary: _summary,
            Year: _year,
            DurationMinutes: _durationMinutes,
            ReleaseDate: _releaseDate,
            Director: _director.Build(),
            Genres: _genres.Select(g => g.Build()).ToList(),
            Actors: _actors.Select(a => a.Build()).ToList(),
            ProductionCountry: _productionCountry.Build()
        );
    }
}
