namespace FilmApi.Models;

public class Director
{
    public string Id { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string Nationality { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }
}
