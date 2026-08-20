using Rentz.Intelligence.Application.Dtos;
using Rentz.Intelligence.Application.Services;

namespace Rentz.Intelligence.Infrastructure.Services;

public class PropertyRankingService : IPropertyRankingService
{
    public Task<List<PropertyRankingResponse>> RankPropertiesAsync(
        List<PropertySearchResponse> properties,
        PropertySearchRequest request)
    {
        var results = properties
            .Select(property =>
            {
                double score = 0;
                var reasons = new List<string>();

                // =====================================================
                // BEDROOM MATCH
                // =====================================================

                if (request.MinBedrooms.HasValue &&
                    property.Bedrooms >= request.MinBedrooms.Value)
                {
                    score += 20;

                    if (property.Bedrooms == request.MinBedrooms.Value)
                    {
                        reasons.Add("Exact bedroom requirement");
                    }
                    else
                    {
                        reasons.Add("Bedroom requirement satisfied");
                    }
                }

                // =====================================================
                // BATHROOM MATCH
                // =====================================================

                if (request.MinBathrooms.HasValue &&
                    property.Bathrooms >= request.MinBathrooms.Value)
                {
                    score += 15;

                    if (property.Bathrooms == request.MinBathrooms.Value)
                    {
                        reasons.Add("Exact bathroom requirement");
                    }
                    else
                    {
                        reasons.Add("Bathroom requirement satisfied");
                    }
                }

                // =====================================================
                // PROPERTY TYPE MATCH
                // =====================================================

                if (!string.IsNullOrWhiteSpace(request.PropertyType) &&
                    property.PropertyType.Equals(
                        request.PropertyType,
                        StringComparison.OrdinalIgnoreCase))
                {
                    score += 15;
                    reasons.Add("Property type matches");
                }

                // =====================================================
                // CITY MATCH
                // =====================================================

                if (!string.IsNullOrWhiteSpace(request.City) &&
                    property.City.Equals(
                        request.City,
                        StringComparison.OrdinalIgnoreCase))
                {
                    score += 20;
                    reasons.Add("City matches");
                }

                // =====================================================
                // BUDGET / VALUE MATCH
                // =====================================================

                if (request.MaxRent.HasValue)
                {
                    var maxRent = request.MaxRent.Value;
                    var rent = property.MonthlyRent;

                    if (rent <= maxRent)
                    {
                        var budgetRatio = rent / maxRent;

                        if (budgetRatio <= 0.80m)
                        {
                            score += 30;
                            reasons.Add("Excellent value within budget");
                        }
                        else if (budgetRatio <= 0.90m)
                        {
                            score += 25;
                            reasons.Add("Good value within budget");
                        }
                        else
                        {
                            score += 20;
                            reasons.Add("Within budget");
                        }
                    }
                }

                // =====================================================
                // FINAL RESULT
                // =====================================================

                return new PropertyRankingResponse
                {
                    PropertyId = property.Id,
                    Score = score,
                    Reason = reasons.Count > 0
                        ? string.Join(", ", reasons)
                        : "Basic property match"
                };
            })
            .OrderByDescending(x => x.Score)
            .ToList();

        return Task.FromResult(results);
    }
}