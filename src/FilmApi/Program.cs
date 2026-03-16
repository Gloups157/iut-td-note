using FilmApi.Models;
using FilmApi.Repositories;
using FilmApi.Services;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Client MongoDB via Aspire (connection name = ressource base dans l'AppHost)
builder.AddMongoDBClient(connectionName: "filmapi");

var pack = new ConventionPack { new CamelCaseElementNameConvention() };
ConventionRegistry.Register("camelCase", pack, _ => true);

// Collection "films" à partir de la base injectée (FILMAPI_DATABASENAME quand lancé par Aspire)
var dbName = builder.Configuration.GetValue<string>("FILMAPI_DATABASENAME")
    ?? builder.Configuration["MongoDb:DatabaseName"]
    ?? "filmapi";
builder.Services.AddSingleton<IMongoCollection<Film>>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(dbName).GetCollection<Film>("films");
});
builder.Services.AddScoped<IFilmRepository, FilmRepository>();
builder.Services.AddScoped<IFilmService, FilmService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Film API",
        Version = "v1",
        Description = "API du TD noté — Films (GET/POST /films), modèle imbriqué, MongoDB."
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Film API v1");
});

app.MapGet("/films", async (
    IFilmService service,
    int page = 1,
    int pageSize = 100,
    CancellationToken ct = default) =>
{
    if (page < 1) page = 1;
    if (pageSize < 1) pageSize = 100;
    var result = await service.GetPagedAsync(page, pageSize, ct);
    return Results.Ok(result);
});

app.MapGet("/films/{id}", async (string id, IFilmService service, CancellationToken ct) =>
{
    var film = await service.GetByIdAsync(id, ct);
    return film is null ? Results.NotFound() : Results.Ok(film);
});

app.MapPost("/films", async (CreateFilmRequest request, IFilmService service, CancellationToken ct) =>
{
    if (string.IsNullOrWhiteSpace(request.Titre))
        return Results.BadRequest("Titre is required.");
    var film = await service.CreateAsync(request, ct);
    return Results.Created($"/films/{film.Id}", film);
});

app.Run();

public partial class Program { }
