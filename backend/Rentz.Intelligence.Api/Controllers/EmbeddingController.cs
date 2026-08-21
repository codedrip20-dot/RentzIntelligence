using Microsoft.AspNetCore.Mvc;
using Rentz.Intelligence.Infrastructure.Services;

namespace Rentz.Intelligence.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmbeddingController : ControllerBase
{
    private readonly EmbeddingService _embeddingService;
    private readonly PropertyEmbeddingService _propertyEmbeddingService;

    public EmbeddingController(
        EmbeddingService embeddingService,
        PropertyEmbeddingService propertyEmbeddingService)
    {
        _embeddingService = embeddingService;
        _propertyEmbeddingService = propertyEmbeddingService;
    }

    // =========================================================
    // TEST SINGLE EMBEDDING
    // =========================================================

    [HttpPost("test")]
    public async Task<IActionResult> TestEmbedding(
        [FromBody] EmbeddingTestRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
        {
            return BadRequest("Text is required.");
        }

        var embedding =
            await _embeddingService.GenerateEmbeddingAsync(
                request.Text,
                cancellationToken);

        return Ok(new
        {
            message = "Embedding generated successfully.",
            dimensions = embedding.Length,
            embedding
        });
    }


    // =========================================================
    // GENERATE PROPERTY EMBEDDINGS
    // =========================================================

    [HttpPost("generate-properties")]
    public async Task<IActionResult> GeneratePropertyEmbeddings(
        CancellationToken cancellationToken)
    {
        var count =
            await _propertyEmbeddingService
                .GeneratePropertyEmbeddingsAsync(
                    cancellationToken);

        return Ok(new
        {
            message = "Property embeddings generated successfully.",
            propertiesProcessed = count
        });
    }
}


// =========================================================
// REQUEST MODEL
// =========================================================

public class EmbeddingTestRequest
{
    public string Text { get; set; } = string.Empty;
}