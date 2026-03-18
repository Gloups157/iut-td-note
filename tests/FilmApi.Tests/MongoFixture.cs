using FilmApi.Models;
using FilmApi.Tests.Builders;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Testcontainers.MongoDb;

namespace FilmApi.Tests;

/// <summary>
/// Fixture xUnit : un conteneur MongoDB partagé pour tous les tests de la classe.
/// À utiliser avec IClassFixture pour les tests d'intégration.
/// </summary>
public sealed class MongoFixture : IAsyncLifetime, IDisposable
{
    private readonly MongoDbContainer _container = new MongoDbBuilder("mongo:7")
        .Build();

    public Task InitializeAsync() => _container.StartAsync();

    public Task DisposeAsync() => _container.DisposeAsync().AsTask();

    public void Dispose() => _container.DisposeAsync().GetAwaiter().GetResult();

    public string GetConnectionString() => _container.GetConnectionString();

    public IMongoCollection<Film> GetCollection(string databaseName = "filmapi")
    {
        var pack = new ConventionPack { new CamelCaseElementNameConvention() };
        ConventionRegistry.Register("camelCase", pack, _ => true);
        var client = new MongoClient(_container.GetConnectionString());
        var database = client.GetDatabase(databaseName);
        return database.GetCollection<Film>("films");
    }

    /// <summary>
    /// Vide la collection films pour isoler les tests.
    /// </summary>
    public async Task ClearFilmsAsync()
    {
        var collection = GetCollection();
        await collection.DeleteManyAsync(FilterDefinition<Film>.Empty);
    }
}
