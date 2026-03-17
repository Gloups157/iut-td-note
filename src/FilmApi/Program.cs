using FilmApi.Models;
using FilmApi.Repositories;
using FilmApi.Services;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("mongodb") ?? throw new InvalidOperationException("Connection string missing. Set ConnectionStrings:mongodb.");

var pack = new ConventionPack { new CamelCaseElementNameConvention() };
ConventionRegistry.Register("camelCase", pack, _ => true);

builder.Services.AddSingleton<IMongoClient>(new MongoClient(connectionString));

var dbName = builder.Configuration.GetValue<string>("FILMAPI_DATABASENAME") ?? builder.Configuration["MongoDb:DatabaseName"];

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
        Description = "API du TD noté — Films (GET/POST/DELETE /films), modèle imbriqué, MongoDB."
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
    int? releaseYear = null) =>
{
    if (page < 1) page = 1;
    if (pageSize < 1) pageSize = 100;
    var result = await service.GetPagedAsync(page, pageSize, releaseYear);
    return Results.Ok(result);
});

app.MapGet("/films/{id}", async (string id, IFilmService service) =>
{
    var film = await service.GetByIdAsync(id);
    return film is null ? Results.NotFound() : Results.Ok(film);
});

app.MapPost("/films", async (CreateFilmRequest request, IFilmService service) =>
{
    if (string.IsNullOrWhiteSpace(request.Title))
        return Results.BadRequest("Title is required.");
    var film = await service.CreateAsync(request);
    return Results.Created($"/films/{film.Id}", film);
});

app.MapDelete("/films/{id}", async (string id, IFilmService service) =>
{
    var deleted = await service.DeleteAsync(id);
    return deleted ? Results.NoContent() : Results.NotFound();
});

app.Run();

public partial class Program { }
