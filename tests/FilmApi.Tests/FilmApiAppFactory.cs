using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace FilmApi.Tests;

/// <summary>
/// Factory pour lancer l'API dans les tests en pointant MongoDB vers le conteneur Testcontainers.
/// </summary>
public sealed class FilmApiAppFactory : WebApplicationFactory<Program>
{
    private readonly MongoFixture _mongo;

    public FilmApiAppFactory(MongoFixture mongo)
    {
        _mongo = mongo;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:mongodb"] = _mongo.GetConnectionString(),
                ["MongoDb:DatabaseName"] = "filmapi"
            });
        });
    }
}
