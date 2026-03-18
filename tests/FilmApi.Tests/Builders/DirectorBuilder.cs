using FilmApi.Models;

namespace FilmApi.Tests.Builders;

public class DirectorBuilder
{
    private string _id = Guid.NewGuid().ToString();
    private string _lastName = string.Empty;
    private string _firstName = string.Empty;
    private string _nationality = string.Empty;
    private DateTime? _birthDate = DateTime.UtcNow;

    public DirectorBuilder WithId(string id)
    {
        _id = id;
        return this;
    }

    public DirectorBuilder WithLastName(string lastName)
    {
        _lastName = lastName;
        return this;
    }

    public DirectorBuilder WithFirstName(string firstName)
    {
        _firstName = firstName;
        return this;
    }

    public DirectorBuilder WithNationality(string nationality)
    {
        _nationality = nationality;
        return this;
    }

    public DirectorBuilder WithBirthDate(DateTime birthDate)
    {
        _birthDate = birthDate;
        return this;
    }

    public Director Build()
    {
        return new Director
        {
            Id = _id,
            LastName = _lastName,
            FirstName = _firstName,
            Nationality = _nationality,
            BirthDate = _birthDate
        };
    }
}