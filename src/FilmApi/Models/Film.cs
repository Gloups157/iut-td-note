using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FilmApi.Models;

public class Film
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    public string Titre { get; set; } = string.Empty;
    public string Resume { get; set; } = string.Empty;
    public int Annee { get; set; }
    public int DureeMinutes { get; set; }
    public DateTime? DateSortie { get; set; }

    public Director Realisateur { get; set; } = null!;
    public List<Genre> Genres { get; set; } = new();
    public List<Actor> Acteurs { get; set; } = new();
    public Country? PaysProduction { get; set; }
}
