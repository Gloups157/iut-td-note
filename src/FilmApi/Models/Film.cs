using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FilmApi.Models;

public class Film
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public int Year { get; set; }
    public int DurationMinutes { get; set; }
    public DateTime? ReleaseDate { get; set; }

    public Director Director { get; set; } = null!;
    public List<Genre> Genres { get; set; } = new();
    public List<Actor> Actors { get; set; } = new();
    public Country? ProductionCountry { get; set; }
}
