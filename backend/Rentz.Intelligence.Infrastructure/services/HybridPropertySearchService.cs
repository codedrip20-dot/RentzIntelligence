using Rentz.Intelligence.Application.Dtos;
using Rentz.Intelligence.Application.Services;

namespace Rentz.Intelligence.Infrastructure.Services;

public class HybridPropertySearchService : IHybridPropertySearchService
{
    private readonly IPropertyService _propertyService;
    private readonly IQueryUnderstandingService _queryUnderstandingService;
    private readonly IPropertyRankingService _propertyRankingService;
    private readonly PropertyVectorSearchService _vectorSearchService;

    public HybridPropertySearchService(
        IPropertyService propertyService,
        IQueryUnderstandingService queryUnderstandingService,
        IPropertyRankingService propertyRankingService,
        PropertyVectorSearchService vectorSearchService)
    {
        _propertyService = propertyService;
        _queryUnderstandingService = queryUnderstandingService;
        _propertyRankingService = propertyRankingService;
        _vectorSearchService = vectorSearchService;
    }

    public async Task<List<HybridPropertySearchResponse>> SearchAsync(
        string query,
        int limit = 10,
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
            limit = 10;
        }

        // =====================================================
        // 1. UNDERSTAND QUERY
        // =====================================================

        var searchRequest =
            await _queryUnderstandingService
                .UnderstandQueryAsync(query);

        // =====================================================
        // 2. STRUCTURED PROPERTY SEARCH
        // =====================================================

        var ruleCandidates =
            await _propertyService
                .SearchPropertiesAsync(searchRequest);

        // =====================================================
        // 3. RULE-BASED RANKING
        // =====================================================

        var rankedRuleCandidates =
            await _propertyRankingService
                .RankPropertiesAsync(
                    ruleCandidates,
                    searchRequest);

        // =====================================================
        // 4. GET ELIGIBLE PROPERTY IDS
        // =====================================================

        var candidateIds =
            ruleCandidates
                .Select(property => property.Id)
                .Distinct()
                .ToList();

        // =====================================================
        // 5. SEMANTIC SEARCH
        // =====================================================

        List<PropertyVectorSearchResult> vectorCandidates;

        if (candidateIds.Count > 0)
        {
            // We have structured candidates.
            // Semantic search is used to rank those candidates.
            vectorCandidates =
                await _vectorSearchService
                    .SearchAsync(
                        query,
                        candidateIds,
                        limit,
                        cancellationToken);
        }
        else
        {
            // No structured candidates were found.
            //
            // This is important for natural-language queries such as:
            //
            // "good room in Gangtok"
            // "cheap room in Gangtok"
            // "quiet place near Gangtok"
            //
            // Let semantic search search across ALL properties.
            vectorCandidates =
                await _vectorSearchService
                    .SearchAsync(
                        query,
                        null,
                        limit,
                        cancellationToken);
        }

        // =====================================================
        // 6. LOOKUP RULE SCORES
        // =====================================================

        var ruleScores =
            rankedRuleCandidates
                .ToDictionary(
                    result => result.PropertyId,
                    result => result);

        // =====================================================
        // 7. LOOKUP PROPERTY DATA
        // =====================================================

        var properties =
            ruleCandidates
                .ToDictionary(
                    property => property.Id);

        // =====================================================
        // 8. DETERMINE SEARCH MODE
        // =====================================================

        var semanticOnlyMode =
            candidateIds.Count == 0;

        // =====================================================
        // 9. BUILD HYBRID RESULTS
        // =====================================================

        var results =
            new List<HybridPropertySearchResponse>();

        foreach (var vectorProperty in vectorCandidates)
        {
            properties.TryGetValue(
                vectorProperty.Id,
                out var property);

            ruleScores.TryGetValue(
                vectorProperty.Id,
                out var ruleScoreResult);

            // =================================================
            // RULE SCORE
            // =================================================

            var ruleScore =
                ruleScoreResult?.Score ?? 0;

            var normalizedRuleScore =
                Math.Clamp(
                    ruleScore,
                    0,
                    100);

            // =================================================
            // SEMANTIC SCORE
            // =================================================

            var similarity =
                1.0 - vectorProperty.Distance;

            similarity =
                Math.Clamp(
                    similarity,
                    0,
                    1);

            var semanticScore =
                similarity * 100;

            // =================================================
            // HYBRID SCORE
            // =================================================

            double hybridScore;

            if (semanticOnlyMode)
            {
                // No structured candidates were available.
                // Semantic relevance becomes the final score.
                hybridScore = semanticScore;
            }
            else
            {
                const double ruleWeight = 0.60;
                const double semanticWeight = 0.40;

                hybridScore =
                    (normalizedRuleScore * ruleWeight) +
                    (semanticScore * semanticWeight);
            }

            // =================================================
            // EXPLAINABLE REASONS
            // =================================================

            var reasons =
                new List<string>();

            if (ruleScoreResult != null &&
                !string.IsNullOrWhiteSpace(
                    ruleScoreResult.Reason))
            {
                reasons.Add(
                    ruleScoreResult.Reason);
            }

            if (semanticOnlyMode)
            {
                reasons.Add(
                    $"Semantic search match: {semanticScore:F1}/100");
            }
            else
            {
                reasons.Add(
                    $"Semantic similarity: {semanticScore:F1}/100");
            }

            // =================================================
            // FINAL RESULT
            // =================================================

            results.Add(
                new HybridPropertySearchResponse
                {
                    Id = vectorProperty.Id,

                    Name =
                        property?.Name ??
                        vectorProperty.Name,

                    Description =
                        property?.Description ??
                        vectorProperty.Description,

                    PropertyType =
                        property?.PropertyType ??
                        vectorProperty.PropertyType,

                    City =
                        property?.City ??
                        vectorProperty.City,

                    State =
                        property?.State ??
                        vectorProperty.State,

                    Country =
                        property?.Country ??
                        vectorProperty.Country,

                    MonthlyRent =
                        property?.MonthlyRent ??
                        vectorProperty.MonthlyRent,

                    Bedrooms =
                        property?.Bedrooms ??
                        vectorProperty.Bedrooms,

                    Bathrooms =
                        property?.Bathrooms ??
                        vectorProperty.Bathrooms,

                    FurnishingType =
                        property?.FurnishingType ??
                        vectorProperty.FurnishingType,

                    RuleScore =
                        normalizedRuleScore,

                    SemanticScore =
                        semanticScore,

                    HybridScore =
                        hybridScore,

                    Reason =
                        string.Join(
                            ", ",
                            reasons)
                });
        }

        // =====================================================
        // 10. FINAL ORDER
        // =====================================================

        return results
            .OrderByDescending(
                result => result.HybridScore)
            .Take(limit)
            .ToList();
    }
}