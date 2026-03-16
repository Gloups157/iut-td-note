using FilmApi.Models;
using FilmApi.Repositories;

namespace FilmApi.Services;

public class FilmService : IFilmService
{
    private readonly IFilmRepository _repository;

    public FilmService(IFilmRepository repository)
    {
        _repository = repository;
    }

    public async Task<Film> CreateAsync(CreateFilmRequest request)
    {
        var film = new Film
        {
            Title = request.Title,
            Summary = request.Summary,
            Year = request.Year,
            DurationMinutes = request.DurationMinutes,
            ReleaseDate = request.ReleaseDate,
            Director = request.Director,
            Genres = request.Genres,
            Actors = request.Actors,
            ProductionCountry = request.ProductionCountry
        };
        return await _repository.AddAsync(film);
    }

    public Task<Film?> GetByIdAsync(string id) => _repository.GetByIdAsync(id);

    public async Task<PagedResult<Film>> GetPagedAsync(int page, int pageSize, int? releaseYear = null)
    {
        var skip = (page - 1) * pageSize;
        var (items, totalCount) = await _repository.GetPagedAsync(skip, pageSize, releaseYear);
        return new PagedResult<Film>(totalCount, page, pageSize, items);
    }
}
