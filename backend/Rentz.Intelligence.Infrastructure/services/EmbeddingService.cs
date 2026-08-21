using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Rentz.Intelligence.Infrastructure.Services;

public class EmbeddingService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    private const string Model = "gemini-embedding-2";
    private const int Dimensions = 768;

    public EmbeddingService(
        HttpClient httpClient,
        string apiKey)
    {
        _httpClient = httpClient;
        _apiKey = apiKey;
    }

    public async Task<float[]> GenerateEmbeddingAsync(
        string text,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException(
                "Text cannot be empty.",
                nameof(text));

        var url =
            $"https://generativelanguage.googleapis.com/v1beta/models/{Model}:embedContent";

        var requestBody = new
        {
            content = new
            {
                parts = new[]
                {
                    new
                    {
                        text
                    }
                }
            },
            output_dimensionality = Dimensions
        };

        var json = JsonSerializer.Serialize(requestBody);

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            url);

        request.Headers.Add(
            "x-goog-api-key",
            _apiKey);

        request.Content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json");

        using var response = await _httpClient.SendAsync(
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var responseJson =
            await response.Content.ReadAsStringAsync(
                cancellationToken);

        using var document =
            JsonDocument.Parse(responseJson);

        var values =
            document.RootElement
                .GetProperty("embedding")
                .GetProperty("values");

        var embedding = new float[values.GetArrayLength()];

        for (int i = 0; i < embedding.Length; i++)
        {
            embedding[i] =
                values[i].GetSingle();
        }

        if (embedding.Length != Dimensions)
        {
            throw new InvalidOperationException(
                $"Expected {Dimensions} dimensions, " +
                $"but received {embedding.Length}.");
        }

        return embedding;
    }
}