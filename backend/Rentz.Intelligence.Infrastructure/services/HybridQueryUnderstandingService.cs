using Rentz.Intelligence.Application.Dtos;
using Rentz.Intelligence.Application.Services;

namespace Rentz.Intelligence.Infrastructure.Services;

public class HybridQueryUnderstandingService : IQueryUnderstandingService
{
    private readonly GeminiQueryUnderstandingService _geminiService;
    private readonly QueryUnderstandingService _fallbackService;

    public HybridQueryUnderstandingService(
        GeminiQueryUnderstandingService geminiService,
        QueryUnderstandingService fallbackService)
    {
        _geminiService = geminiService;
        _fallbackService = fallbackService;
    }

    public async Task<PropertySearchRequest> UnderstandQueryAsync(
        string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return new PropertySearchRequest();
        }

        // =====================================================
        // ATTEMPT 1 — GEMINI
        // =====================================================

        try
        {
            Console.WriteLine(
                "[HybridQuery] Gemini attempt 1..."
            );

            var request = await _geminiService
                .UnderstandQueryAsync(query);

            request.Source = QueryUnderstandingSource.Gemini;

            Console.WriteLine(
                "[HybridQuery] Gemini attempt 1 succeeded."
            );

            Console.WriteLine(
                "[HybridQuery] Source: Gemini"
            );

            return request;
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"[HybridQuery] Gemini attempt 1 failed: {ex.Message}"
            );
        }

        // =====================================================
        // ATTEMPT 2 — GEMINI RETRY
        // =====================================================

        try
        {
            Console.WriteLine(
                "[HybridQuery] Waiting before Gemini retry..."
            );

            await Task.Delay(1000);

            Console.WriteLine(
                "[HybridQuery] Gemini attempt 2..."
            );

            var request = await _geminiService
                .UnderstandQueryAsync(query);

            request.Source = QueryUnderstandingSource.GeminiRetry;

            Console.WriteLine(
                "[HybridQuery] Gemini attempt 2 succeeded."
            );

            Console.WriteLine(
                "[HybridQuery] Source: GeminiRetry"
            );

            return request;
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"[HybridQuery] Gemini attempt 2 failed: {ex.Message}"
            );
        }

        // =====================================================
        // FALLBACK — REGEX / RULE BASED
        // =====================================================

        Console.WriteLine(
            "[HybridQuery] Gemini unavailable."
        );

        Console.WriteLine(
            "[HybridQuery] Using rule-based fallback..."
        );

        var fallbackRequest = await _fallbackService
            .UnderstandQueryAsync(query);

        fallbackRequest.Source =
            QueryUnderstandingSource.RegexFallback;

        Console.WriteLine(
            "[HybridQuery] Source: RegexFallback"
        );

        return fallbackRequest;
    }
}