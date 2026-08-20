namespace Rentz.Intelligence.Application.Dtos;

public class PropertyDetailsResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string PropertyType { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public decimal MonthlyRent { get; set; }

    public decimal SecurityDeposit { get; set; }

    public int Bedrooms { get; set; }

    public int Bathrooms { get; set; }

    public string FurnishingType { get; set; } = string.Empty;

    public List<string> Amenities { get; set; } = new();

    public List<PropertyImageResponse> Images { get; set; } = new();

    public List<PropertyDocumentResponse> Documents { get; set; } = new();
}

public class PropertyImageResponse
{
    public Guid Id { get; set; }

    public string Url { get; set; } = string.Empty;

    public string AltText { get; set; } = string.Empty;
}

public class PropertyDocumentResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string DocumentType { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;
}