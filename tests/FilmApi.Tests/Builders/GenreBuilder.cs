using FilmApi.Models;

namespace FilmApi.Tests.Builders;

public class GenreBuilder
{
    private string _id = Guid.NewGuid().ToString();
    private string _name = string.Empty;

    public GenreBuilder WithId(string id)
    {
        _id = id;
        return this;
    }

    public GenreBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public Genre Build()
    {
        return new Genre
        {
            Id = _id,
            Name = _name
        };
    }
}
