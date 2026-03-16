namespace FilmApi.Models;

public class Director
{
    public string Id { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Nationalite { get; set; } = string.Empty;
    public DateTime? DateNaissance { get; set; }
}
