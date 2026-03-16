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

    public async Task<Film> CreateAsync(CreateFilmRequest request, CancellationToken ct = default)
    {
        var film = new Film
        {
            Titre = request.Titre,
            Resume = request.Resume,
            Annee = request.Annee,
            DureeMinutes = request.DureeMinutes,
            DateSortie = request.DateSortie,
            Realisateur = request.Realisateur,
            Genres = request.Genres,
            Acteurs = request.Acteurs,
            PaysProduction = request.PaysProduction
        };
        return await _repository.AddAsync(film, ct);
    }

    public Task<Film?> GetByIdAsync(string id, CancellationToken ct = default)
        => _repository.GetByIdAsync(id, ct);

    public async Task<PagedResult<Film>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var skip = (page - 1) * pageSize;
        var (items, totalCount) = await _repository.GetPagedAsync(skip, pageSize, ct);
        return new PagedResult<Film>(totalCount, page, pageSize, items);
    }
}
