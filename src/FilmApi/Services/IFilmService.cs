using FilmApi.Models;

namespace FilmApi.Services;

public interface IFilmService
{
    Task<Film> CreateAsync(CreateFilmRequest request, CancellationToken ct = default);
    Task<Film?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<PagedResult<Film>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
}
