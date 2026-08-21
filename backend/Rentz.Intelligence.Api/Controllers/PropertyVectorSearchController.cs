using Microsoft.AspNetCore.Mvc;
using Rentz.Intelligence.Infrastructure.Services;

namespace Rentz.Intelligence.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropertyVectorSearchController : ControllerBase
{
    private readonly PropertyVectorSearchService _searchService;

    public PropertyVectorSearchController(
        PropertyVectorSearchService searchService)
    {
        _searchService = searchService;
    }

    // =========================================================
    // SEMANTIC PROPERTY SEARCH
    // =========================================================

    [HttpPost("search")]
    public async Task<IActionResult> Search(
        [FromBody] PropertyVectorSearchRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
        {
            return BadRequest("Query is required.");
        }

        var results =
            await _searchService.SearchAsync(
                request.Query,
                request.Limit,
                cancellationToken);

        return Ok(new
        {
            query = request.Query,
            count = results.Count,
            results
        });
    }
}


// =========================================================
// REQUEST MODEL
// =========================================================

public class PropertyVectorSearchRequest
{
    public string Query { get; set; } = string.Empty;

    public int Limit { get; set; } = 5;
}