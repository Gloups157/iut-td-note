using FilmApi.Models;

namespace FilmApi.Repositories;

public interface IFilmRepository
{
    Task<Film> AddAsync(Film film);
    Task<Film?> GetByIdAsync(string id);
    Task<(IReadOnlyList<Film> Items, int TotalCount)> GetPagedAsync(int skip, int take, int? releaseYear = null);
    Task<bool> DeleteByIdAsync(string id);
}
