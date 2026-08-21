namespace Rentz.Intelligence.Application.Dtos;

public class PropertySearchRequest
{
    public string? City { get; set; }

    public decimal? MinRent { get; set; }

    public decimal? MaxRent { get; set; }

    public int? MinBedrooms { get; set; }

    public int? MinBathrooms { get; set; }

    public string? PropertyType { get; set; }

    public List<string> Amenities { get; set; } = new();

    // How the query was understood
    public QueryUnderstandingSource Source { get; set; }
}