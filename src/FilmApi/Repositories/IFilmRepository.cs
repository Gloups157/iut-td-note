using FilmApi.Models;

namespace FilmApi.Repositories;

public interface IFilmRepository
{
    Task<Film> AddAsync(Film film, CancellationToken ct = default);
    Task<Film?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<(IReadOnlyList<Film> Items, int TotalCount)> GetPagedAsync(int skip, int take, CancellationToken ct = default);
}
