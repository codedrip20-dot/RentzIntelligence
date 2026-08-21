namespace Rentz.Intelligence.Application.Dtos;

public class QueryUnderstandingResult
{
    public PropertySearchRequest Request { get; set; } = new();

    public QueryUnderstandingSource Source { get; set; }

    public int Attempts { get; set; }
}