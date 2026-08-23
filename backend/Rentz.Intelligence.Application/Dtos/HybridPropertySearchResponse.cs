namespace Rentz.Intelligence.Application.Dtos;

public class HybridPropertySearchResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string PropertyType { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public decimal MonthlyRent { get; set; }

    public int Bedrooms { get; set; }

    public int Bathrooms { get; set; }

    public string FurnishingType { get; set; } = string.Empty;

    // Rule-based ranking score, normalized to 0-100
    public double RuleScore { get; set; }

    // Semantic similarity score, normalized to 0-100
    public double SemanticScore { get; set; }

    // Final hybrid score, 0-100
    public double HybridScore { get; set; }

    public string Reason { get; set; } = string.Empty;
}