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
            PropertyType = ExtractPropertyType(normalizedQuery),
            FurnishingType = ExtractFurnishingType(normalizedQuery),
            Amenities = ExtractAmenities(normalizedQuery)
        };

        return Task.FromResult(request);
    }

    // =========================================================
    // CITY
    // =========================================================

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
            query.Contains(
                city,
                StringComparison.OrdinalIgnoreCase));
    }

    // =========================================================
    // MINIMUM RENT
    // =========================================================

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
        // Also handles:
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

    // =========================================================
    // MAXIMUM RENT
    // =========================================================

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
        // Also handles:
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

    // =========================================================
    // BEDROOMS
    // =========================================================

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

    // =========================================================
    // BATHROOMS
    // =========================================================

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

    // =========================================================
    // PROPERTY TYPE
    // =========================================================

    private static string? ExtractPropertyType(string query)
    {
        var propertyTypes = new[]
        {
            "Apartment",
            "House"
        };

        return propertyTypes.FirstOrDefault(type =>
            query.Contains(
                type,
                StringComparison.OrdinalIgnoreCase));
    }

    // =========================================================
    // FURNISHING TYPE
    // =========================================================

    private static string? ExtractFurnishingType(string query)
    {
        // Fully furnished
        if (query.Contains(
                "fully furnished",
                StringComparison.OrdinalIgnoreCase) ||
            query.Contains(
                "fully-furnished",
                StringComparison.OrdinalIgnoreCase))
        {
            return "Fully Furnished";
        }

        // Semi furnished
        if (query.Contains(
                "semi furnished",
                StringComparison.OrdinalIgnoreCase) ||
            query.Contains(
                "semi-furnished",
                StringComparison.OrdinalIgnoreCase))
        {
            return "Semi Furnished";
        }

        // Unfurnished
        if (query.Contains(
                "unfurnished",
                StringComparison.OrdinalIgnoreCase) ||
            query.Contains(
                "un-furnished",
                StringComparison.OrdinalIgnoreCase))
        {
            return "Unfurnished";
        }

        return null;
    }

    // =========================================================
    // AMENITIES
    // =========================================================

    private static List<string> ExtractAmenities(string query)
    {
        var amenities = new Dictionary<string, string[]>(
            StringComparer.OrdinalIgnoreCase)
        {
            ["Wi-Fi"] = new[]
            {
                "wifi",
                "wi-fi",
                "wi fi",
                "internet"
            },

            ["Parking"] = new[]
            {
                "parking",
                "car parking",
                "vehicle parking"
            },

            ["Power Backup"] = new[]
            {
                "power backup",
                "backup power",
                "electricity backup",
                "backup electricity",
                "generator"
            },

            ["24/7 Security"] = new[]
            {
                "security",
                "24/7 security",
                "24 7 security",
                "security guard"
            },

            ["Gym"] = new[]
            {
                "gym",
                "fitness center",
                "fitness centre"
            },

            ["Swimming Pool"] = new[]
            {
                "swimming pool",
                "pool"
            },

            ["Balcony"] = new[]
            {
                "balcony",
                "balconies"
            },

            ["Fully Equipped Kitchen"] = new[]
            {
                "fully equipped kitchen",
                "equipped kitchen",
                "modular kitchen"
            },

            ["Laundry"] = new[]
            {
                "laundry",
                "washing machine"
            },

            ["CCTV"] = new[]
            {
                "cctv",
                "surveillance",
                "security cameras"
            },

            ["Elevator"] = new[]
            {
                "elevator",
                "lift"
            },

            ["Pet Friendly"] = new[]
            {
                "pet friendly",
                "pets allowed",
                "allows pets",
                "pet-friendly"
            },

            ["Garden"] = new[]
            {
                "garden",
                "backyard",
                "green space"
            },

            ["Rooftop"] = new[]
            {
                "rooftop",
                "roof top",
                "terrace"
            },

            ["Housekeeping"] = new[]
            {
                "housekeeping",
                "cleaning service",
                "cleaning services"
            }
        };

        var matchedAmenities = new List<string>();

        foreach (var amenity in amenities)
        {
            var found = amenity.Value.Any(keyword =>
                query.Contains(
                    keyword,
                    StringComparison.OrdinalIgnoreCase));

            if (found)
            {
                matchedAmenities.Add(amenity.Key);
            }
        }

        return matchedAmenities;
    }

    // =========================================================
    // RENT PARSER
    // =========================================================

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

        if (suffix.Equals(
                "k",
                StringComparison.OrdinalIgnoreCase))
        {
            amount *= 1000;
        }

        return amount;
    }
}