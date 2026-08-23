using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;
using Rentz.Intelligence.Domain.Entities;
using Rentz.Intelligence.Infrastructure.Persistence;

namespace Rentz.Intelligence.Infrastructure.Services;

public class PropertyVectorSearchService
{
    private readonly RentzDbContext _db;
    private readonly EmbeddingService _embeddingService;

    public PropertyVectorSearchService(
        RentzDbContext db,
        EmbeddingService embeddingService)
    {
        _db = db;
        _embeddingService = embeddingService;
    }

    public async Task<List<PropertyVectorSearchResult>> SearchAsync(
        string query,
        int limit = 5,
        CancellationToken cancellationToken = default)
    {
        return await SearchAsync(
            query,
            null,
            limit,
            cancellationToken);
    }

    public async Task<List<PropertyVectorSearchResult>> SearchAsync(
        string query,
        IReadOnlyCollection<Guid>? candidateIds,
        int limit = 5,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            throw new ArgumentException(
                "Search query cannot be empty.",
                nameof(query));
        }

        if (limit <= 0)
        {
            limit = 5;
        }

        // =====================================================
        // 1. GENERATE QUERY EMBEDDING
        // =====================================================

        var embedding =
            await _embeddingService.GenerateEmbeddingAsync(
                query,
                cancellationToken);

        var queryVector = new Vector(embedding);

        // =====================================================
        // 2. START VECTOR QUERY
        // =====================================================

        var properties = _db.Properties
            .Where(p => p.Embedding != null);

        // =====================================================
        // 3. APPLY STRUCTURED CANDIDATE FILTER
        // =====================================================

        if (candidateIds != null)
        {
            if (candidateIds.Count == 0)
            {
                return new List<PropertyVectorSearchResult>();
            }

            properties = properties
                .Where(p => candidateIds.Contains(p.Id));
        }

        // =====================================================
        // 4. SEMANTIC SEARCH
        // =====================================================

        var results = await properties
            .OrderBy(p =>
                p.Embedding!.CosineDistance(queryVector))
            .Take(limit)
            .Select(p => new PropertyVectorSearchResult
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                PropertyType = p.PropertyType,
                City = p.City,
                State = p.State,
                Country = p.Country,
                MonthlyRent = p.MonthlyRent,
                SecurityDeposit = p.SecurityDeposit,
                Bedrooms = p.Bedrooms,
                Bathrooms = p.Bathrooms,
                FurnishingType = p.FurnishingType,

                Distance =
                    p.Embedding!.CosineDistance(queryVector)
            })
            .ToListAsync(cancellationToken);

        return results;
    }
}


// =========================================================
// VECTOR SEARCH RESULT
// =========================================================

public class PropertyVectorSearchResult
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

    public double Distance { get; set; }
}