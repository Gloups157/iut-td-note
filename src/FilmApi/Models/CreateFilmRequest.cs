namespace FilmApi.Models;

public record CreateFilmRequest(
    string Titre,
    string Resume,
    int Annee,
    int DureeMinutes,
    DateTime? DateSortie,
    Director Realisateur,
    List<Genre> Genres,
    List<Actor> Acteurs,
    Country? PaysProduction
);
