using Microsoft.EntityFrameworkCore;
using Pgvector;
using Rentz.Intelligence.Infrastructure.Persistence;

namespace Rentz.Intelligence.Infrastructure.Services;

public class PropertyEmbeddingService
{
    private readonly RentzDbContext _db;
    private readonly EmbeddingService _embeddingService;

    public PropertyEmbeddingService(
        RentzDbContext db,
        EmbeddingService embeddingService)
    {
        _db = db;
        _embeddingService = embeddingService;
    }

    public async Task<int> GeneratePropertyEmbeddingsAsync(
        CancellationToken cancellationToken = default)
    {
        var properties = await _db.Properties
            .ToListAsync(cancellationToken);

        var count = 0;

        foreach (var property in properties)
        {
            var searchableText = BuildSearchableText(property);

            var embedding =
                await _embeddingService.GenerateEmbeddingAsync(
                    searchableText,
                    cancellationToken);

            property.SetEmbedding(
                new Vector(embedding));

            count++;
        }

        await _db.SaveChangesAsync(cancellationToken);

        return count;
    }

    private static string BuildSearchableText(
        Rentz.Intelligence.Domain.Entities.Property property)
    {
        return $"""
            Property name: {property.Name}
            Description: {property.Description}
            Property type: {property.PropertyType}
            Location: {property.City}, {property.State}, {property.Country}
            Monthly rent: {property.MonthlyRent}
            Bedrooms: {property.Bedrooms}
            Bathrooms: {property.Bathrooms}
            Furnishing type: {property.FurnishingType}
            """;
    }
}