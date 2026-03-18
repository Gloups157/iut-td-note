using FilmApi.Models;

namespace FilmApi.Tests.Builders;

public class ActorBuilder
{
    private string _id = Guid.NewGuid().ToString();
    private string _lastName = string.Empty;
    private string _firstName = string.Empty;
    private string _role = string.Empty;

    public ActorBuilder WithId(string id)
    {
        _id = id;
        return this;
    }

    public ActorBuilder WithLastName(string lastName)
    {
        _lastName = lastName;
        return this;
    }

    public ActorBuilder WithFirstName(string firstName)
    {
        _firstName = firstName;
        return this;
    }

    public ActorBuilder WithRole(string role)
    {
        _role = role;
        return this;
    }

    public Actor Build()
    {
        return new Actor
        {
            Id = _id,
            LastName = _lastName,
            FirstName = _firstName,
            Role = _role
        };
    }
}
