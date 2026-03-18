namespace FilmApi.Models;

public class CountryBuilder
{
    private string _code = "33";
    private string _name = "Country";

    public CountryBuilder WithCode(string? code)
    {
        _code = code;
        return this;
    }

    public CountryBuilder WithName(string? name)
    {
        _name = name;
        return this;
    }
    
    public Country Build()
    {
        return  new Country
        {
            Code = _code,
            Name = _name
        };
    }
}
