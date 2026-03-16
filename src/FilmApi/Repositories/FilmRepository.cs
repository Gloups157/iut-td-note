using FilmApi.Models;
using MongoDB.Driver;

namespace FilmApi.Repositories;

public class FilmRepository : IFilmRepository
{
    private readonly IMongoCollection<Film> _collection;

    public FilmRepository(IMongoCollection<Film> collection)
    {
        _collection = collection;
    }

    public async Task<Film> AddAsync(Film film, CancellationToken ct = default)
    {
        await _collection.InsertOneAsync(film, cancellationToken: ct);
        return film;
    }

    public async Task<Film?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        var cursor = await _collection.FindAsync(f => f.Id == id, cancellationToken: ct);
        return await cursor.FirstOrDefaultAsync(ct);
    }

    public async Task<(IReadOnlyList<Film> Items, int TotalCount)> GetPagedAsync(int skip, int take, CancellationToken ct = default)
    {
        var totalCount = await _collection.CountDocumentsAsync(FilterDefinition<Film>.Empty, cancellationToken: ct);
        var items = await _collection
            .Find(FilterDefinition<Film>.Empty)
            .SortBy(f => f.Id)
            .Skip(skip)
            .Limit(take)
            .ToListAsync(ct);
        return (items, (int)totalCount);
    }
}
