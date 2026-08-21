using Pgvector;

namespace Rentz.Intelligence.Domain.Entities;

public class Property
{
    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public string PropertyType { get; private set; } = string.Empty;

    public string City { get; private set; } = string.Empty;

    public string State { get; private set; } = string.Empty;

    public string Country { get; private set; } = string.Empty;

    public decimal MonthlyRent { get; private set; }

    public decimal SecurityDeposit { get; private set; }

    public int Bedrooms { get; private set; }

    public int Bathrooms { get; private set; }

    public string FurnishingType { get; private set; } = string.Empty;

    // AI-generated semantic embedding
    public Vector? Embedding { get; private set; }

    public ICollection<PropertyAmenity> Amenities { get; private set; }
        = new List<PropertyAmenity>();


    // =========================================================
    // CREATE PROPERTY
    // =========================================================

    public static Property Create(
        Guid id,
        string name,
        string description,
        string propertyType,
        string city,
        string state,
        string country,
        decimal monthlyRent,
        decimal securityDeposit,
        int bedrooms,
        int bathrooms,
        string furnishingType)
    {
        return new Property
        {
            Id = id,
            Name = name,
            Description = description,
            PropertyType = propertyType,
            City = city,
            State = state,
            Country = country,
            MonthlyRent = monthlyRent,
            SecurityDeposit = securityDeposit,
            Bedrooms = bedrooms,
            Bathrooms = bathrooms,
            FurnishingType = furnishingType
        };
    }


    // =========================================================
    // EMBEDDING
    // =========================================================

    public void SetEmbedding(Vector embedding)
    {
        Embedding = embedding;
    }


    // =========================================================
    // EF CORE
    // =========================================================

    private Property()
    {
    }
}