using FilmApi.Models;

namespace FilmApi.Services;

public interface IFilmService
{
    Task<Film> CreateAsync(CreateFilmRequest request);
    Task<Film?> GetByIdAsync(string id);
    Task<PagedResult<Film>> GetPagedAsync(int page, int pageSize, int? releaseYear = null);
    Task<bool> DeleteAsync(string id);
}
