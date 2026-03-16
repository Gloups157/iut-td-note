namespace FilmApi.Models;

public record PagedResult<T>(int TotalCount, int Page, int PageSize, IReadOnlyList<T> Items);
