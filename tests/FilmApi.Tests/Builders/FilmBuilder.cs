using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using FilmApi.Models;

namespace FilmApi.Tests.Builders;

public class FilmBuilder
{
    private string _id = Guid.NewGuid().ToString();
    private string _title = string.Empty;
    private string _summary = string.Empty;
    private int _year;
    private int _durationMinutes;
    private DateTime? _releaseDate = DateTime.UtcNow;
    private DirectorBuilder _directorBuilder = new();
    private List<GenreBuilder> _genreBuilders = new();
    private List<ActorBuilder> _actorBuilders = new();
    private CountryBuilder? _countryBuilder;

    public FilmBuilder WithId(string id)
    {
        _id = id;
        return this;
    }

    public FilmBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public FilmBuilder WithSummary(string summary)
    {
        _summary = summary;
        return this;
    }

    public FilmBuilder WithYear(int year)
    {
        _year = year;
        return this;
    }

    public FilmBuilder WithDurationMinutes(int durationMinutes)
    {
        _durationMinutes = durationMinutes;
        return this;
    }

    public FilmBuilder WithReleaseDate(DateTime? releaseDate)
    {
        _releaseDate = releaseDate;
        return this;
    }

    public FilmBuilder WithDirector(DirectorBuilder directorBuilder)
    {
        _directorBuilder = directorBuilder;
        return this;
    }

    public FilmBuilder AddGenre(GenreBuilder genreBuilder)
    {
        _genreBuilders.Add(genreBuilder);
        return this;
    }

    public FilmBuilder AddActor(ActorBuilder actorBuilder)
    {
        _actorBuilders.Add(actorBuilder);
        return this;
    }

    public FilmBuilder WithCountry(CountryBuilder? countryBuilder)
    {
        _countryBuilder = countryBuilder;
        return this;
    }

    public FilmBuilder WithoutGenres()
    {
        _genreBuilders.Clear();
        return this;
    }

    public FilmBuilder WithoutActors()
    {
        _actorBuilders.Clear();
        return this;
    }

    public Film Build()
    {
        var director = _directorBuilder.Build();
        var genres = _genreBuilders.Select(gb => gb.Build()).ToList();
        var actors = _actorBuilders.Select(ab => ab.Build()).ToList();
        var country = _countryBuilder?.Build();

        return new Film
        {
            Id = _id,
            Title = _title,
            Summary = _summary,
            Year = _year,
            DurationMinutes = _durationMinutes,
            ReleaseDate = _releaseDate,
            Director = director,
            Genres = genres,
            Actors = actors,
            ProductionCountry = country
        };
    }
}
