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

    public async Task<Film> AddAsync(Film film)
    {
        await _collection.InsertOneAsync(film);
        return film;
    }

    public async Task<Film?> GetByIdAsync(string id)
    {
        var cursor = await _collection.FindAsync(f => f.Id == id);
        return await cursor.FirstOrDefaultAsync();
    }

    public async Task<(IReadOnlyList<Film> Items, int TotalCount)> GetPagedAsync(int skip, int take, int? releaseYear = null)
    {
        var filter = releaseYear.HasValue
            ? Builders<Film>.Filter.And(
                Builders<Film>.Filter.Gte(f => f.ReleaseDate, new DateTime(releaseYear.Value, 1, 1)),
                Builders<Film>.Filter.Lt(f => f.ReleaseDate, new DateTime(releaseYear.Value + 1, 1, 1)))
            : FilterDefinition<Film>.Empty;
        var totalCount = await _collection.CountDocumentsAsync(filter);
        var items = await _collection
            .Find(filter)
            .SortBy(f => f.Id)
            .Skip(skip)
            .Limit(take)
            .ToListAsync();
        return (items, (int)totalCount);
    }
}
