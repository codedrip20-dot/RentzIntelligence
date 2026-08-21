using System.Text.Json;
using Google.GenAI;
using GeminiTypes = Google.GenAI.Types;
using Rentz.Intelligence.Application.Dtos;
using Rentz.Intelligence.Application.Services;

namespace Rentz.Intelligence.Infrastructure.Services;

public class GeminiQueryUnderstandingService : IQueryUnderstandingService
{
    private readonly Client _client;

    public GeminiQueryUnderstandingService(string apiKey)
    {
        _client = new Client(apiKey: apiKey);
    }

    public async Task<PropertySearchRequest> UnderstandQueryAsync(
        string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return new PropertySearchRequest();
        }

        var prompt = $"""
            You are a property search query understanding system.

            Extract property search requirements from the user's query.

            Rules:
            - Only extract information explicitly stated or clearly implied.
            - If a value is not present, return null for that field.
            - Do not invent cities, prices, bedrooms, bathrooms,
              property types, or amenities.
            - Rent values must be returned in Indian rupees.
            - "20k" means 20000.
            - "25k" means 25000.
            - For a rent range, populate both MinRent and MaxRent.
            - "under 25000" means MaxRent = 25000.
            - "below 25000" means MaxRent = 25000.
            - "up to 25000" means MaxRent = 25000.
            - "above 20000" means MinRent = 20000.
            - "over 20000" means MinRent = 20000.
            - "at least 20000" means MinRent = 20000.
            - BHK means bedrooms.
            - Return only the requested structured information.

            User query:
            {query}
            """;

        var config = new GeminiTypes.GenerateContentConfig
        {
            ResponseMimeType = "application/json",
            ResponseSchema = BuildSchema()
        };

        var response = await _client.Models.GenerateContentAsync(
            model: "gemini-3.6-flash",
            contents: prompt,
            config: config);

        var json = response.Candidates?
            .FirstOrDefault()?
            .Content?
            .Parts?
            .FirstOrDefault()?
            .Text;

        if (string.IsNullOrWhiteSpace(json))
        {
            throw new InvalidOperationException(
                "Gemini returned an empty response.");
        }

        // =====================================================
        // DEBUG — SHOW GEMINI RAW RESPONSE
        // =====================================================

        Console.WriteLine();
        Console.WriteLine("========================================");
        Console.WriteLine("GEMINI RAW RESPONSE");
        Console.WriteLine("========================================");
        Console.WriteLine(json);
        Console.WriteLine("========================================");
        Console.WriteLine();

        // =====================================================
        // DESERIALIZE GEMINI RESPONSE
        // =====================================================

        var result =
            JsonSerializer.Deserialize<PropertySearchRequest>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

        if (result == null)
        {
            throw new InvalidOperationException(
                "Gemini response could not be converted into PropertySearchRequest.");
        }

        // =====================================================
        // DEBUG — SHOW PARSED SEARCH REQUEST
        // =====================================================

        Console.WriteLine("========================================");
        Console.WriteLine("PARSED PROPERTY SEARCH REQUEST");
        Console.WriteLine("========================================");

        Console.WriteLine($"City: {result.City}");
        Console.WriteLine($"MinRent: {result.MinRent}");
        Console.WriteLine($"MaxRent: {result.MaxRent}");
        Console.WriteLine($"MinBedrooms: {result.MinBedrooms}");
        Console.WriteLine($"MinBathrooms: {result.MinBathrooms}");
        Console.WriteLine($"PropertyType: {result.PropertyType}");

        Console.WriteLine(
            $"Amenities: {string.Join(", ", result.Amenities)}"
        );

        Console.WriteLine("========================================");
        Console.WriteLine();

        return result;
    }

    // =========================================================
    // GEMINI RESPONSE SCHEMA
    // =========================================================

    private static GeminiTypes.Schema BuildSchema()
    {
        return new GeminiTypes.Schema
        {
            Type = "OBJECT",

            Properties = new Dictionary<string, GeminiTypes.Schema>
            {
                ["city"] = new GeminiTypes.Schema
                {
                    Type = "STRING",
                    Nullable = true
                },

                ["minRent"] = new GeminiTypes.Schema
                {
                    Type = "NUMBER",
                    Nullable = true
                },

                ["maxRent"] = new GeminiTypes.Schema
                {
                    Type = "NUMBER",
                    Nullable = true
                },

                ["minBedrooms"] = new GeminiTypes.Schema
                {
                    Type = "INTEGER",
                    Nullable = true
                },

                ["minBathrooms"] = new GeminiTypes.Schema
                {
                    Type = "INTEGER",
                    Nullable = true
                },

                ["propertyType"] = new GeminiTypes.Schema
                {
                    Type = "STRING",
                    Nullable = true
                },

                ["amenities"] = new GeminiTypes.Schema
                {
                    Type = "ARRAY",

                    Items = new GeminiTypes.Schema
                    {
                        Type = "STRING"
                    }
                }
            }
        };
    }
}