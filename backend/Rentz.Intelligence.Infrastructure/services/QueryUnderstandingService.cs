using System.Globalization;
using System.Text.RegularExpressions;
using Rentz.Intelligence.Application.Dtos;
using Rentz.Intelligence.Application.Services;

namespace Rentz.Intelligence.Infrastructure.Services;

public class QueryUnderstandingService : IQueryUnderstandingService
{
    public Task<PropertySearchRequest> UnderstandQueryAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return Task.FromResult(new PropertySearchRequest());
        }

        var normalizedQuery = query.Trim();

        var request = new PropertySearchRequest
        {
            City = ExtractCity(normalizedQuery),
            MinRent = ExtractMinRent(normalizedQuery),
            MaxRent = ExtractMaxRent(normalizedQuery),
            MinBedrooms = ExtractMinBedrooms(normalizedQuery),
            MinBathrooms = ExtractMinBathrooms(normalizedQuery),
            PropertyType = ExtractPropertyType(normalizedQuery)
        };

        return Task.FromResult(request);
    }

    private static string? ExtractCity(string query)
    {
        var cities = new[]
        {
            "Gangtok",
            "Namchi",
            "Pelling",
            "Siliguri",
            "Kolkata",
            "Nainital"
        };

        return cities.FirstOrDefault(city =>
            query.Contains(city, StringComparison.OrdinalIgnoreCase));
    }

    private static decimal? ExtractMinRent(string query)
    {
        // Handles:
        // above 20000
        // over 20000
        // minimum 20000
        // min 20000
        // at least 20000
        // above 20k
        // over ₹20,000
        //
        // Also handles ranges:
        // between 15000 and 25000
        // between 15k and 25k
        // from 15k to 25k
        // 15k - 25k

        var rangeMatch = Regex.Match(
            query,
            @"(?:between|from)\s*(?:₹|rs\.?|inr)?\s*([\d,.]+)\s*(k)?\s*(?:and|to|-)\s*(?:₹|rs\.?|inr)?\s*([\d,.]+)\s*(k)?",
            RegexOptions.IgnoreCase);

        if (rangeMatch.Success)
        {
            return ParseRent(
                rangeMatch.Groups[1].Value,
                rangeMatch.Groups[2].Value);
        }

        var match = Regex.Match(
            query,
            @"(?:above|over|minimum|min|at least)\s*(?:₹|rs\.?|inr)?\s*([\d,.]+)\s*(k)?",
            RegexOptions.IgnoreCase);

        if (!match.Success)
        {
            return null;
        }

        return ParseRent(
            match.Groups[1].Value,
            match.Groups[2].Value);
    }

    private static decimal? ExtractMaxRent(string query)
    {
        // Handles:
        // under 25000
        // below 25000
        // less than 25000
        // max 25000
        // maximum 25000
        // up to 25k
        // under ₹25,000
        //
        // Also handles ranges:
        // between 15000 and 25000
        // between 15k and 25k
        // from 15k to 25k
        // 15k - 25k

        var rangeMatch = Regex.Match(
            query,
            @"(?:between|from)\s*(?:₹|rs\.?|inr)?\s*([\d,.]+)\s*(k)?\s*(?:and|to|-)\s*(?:₹|rs\.?|inr)?\s*([\d,.]+)\s*(k)?",
            RegexOptions.IgnoreCase);

        if (rangeMatch.Success)
        {
            return ParseRent(
                rangeMatch.Groups[3].Value,
                rangeMatch.Groups[4].Value);
        }

        var match = Regex.Match(
            query,
            @"(?:under|below|less than|max|maximum|up to)\s*(?:₹|rs\.?|inr)?\s*([\d,.]+)\s*(k)?",
            RegexOptions.IgnoreCase);

        if (!match.Success)
        {
            return null;
        }

        return ParseRent(
            match.Groups[1].Value,
            match.Groups[2].Value);
    }

    private static int? ExtractMinBedrooms(string query)
    {
        var match = Regex.Match(
            query,
            @"(\d+)\s*(?:bedroom|bedrooms|bhk)",
            RegexOptions.IgnoreCase);

        if (!match.Success)
        {
            return null;
        }

        return int.Parse(match.Groups[1].Value);
    }

    private static int? ExtractMinBathrooms(string query)
    {
        var match = Regex.Match(
            query,
            @"(\d+)\s*(?:bathroom|bathrooms|bath)",
            RegexOptions.IgnoreCase);

        if (!match.Success)
        {
            return null;
        }

        return int.Parse(match.Groups[1].Value);
    }

    private static string? ExtractPropertyType(string query)
    {
        var propertyTypes = new[]
        {
            "Apartment",
            "House"
        };

        return propertyTypes.FirstOrDefault(type =>
            query.Contains(type, StringComparison.OrdinalIgnoreCase));
    }

    private static decimal? ParseRent(
        string value,
        string suffix)
    {
        value = value.Replace(",", "");

        if (!decimal.TryParse(
                value,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out var amount))
        {
            return null;
        }

        if (suffix.Equals("k", StringComparison.OrdinalIgnoreCase))
        {
            amount *= 1000;
        }

        return amount;
    }
}