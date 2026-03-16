namespace FilmApi.Models;

public record CreateFilmRequest(
    string Title,
    string Summary,
    int Year,
    int DurationMinutes,
    DateTime? ReleaseDate,
    Director Director,
    List<Genre> Genres,
    List<Actor> Actors,
    Country? ProductionCountry
);
