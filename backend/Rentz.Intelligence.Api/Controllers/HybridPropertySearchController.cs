using Microsoft.AspNetCore.Mvc;
using Rentz.Intelligence.Application.Services;

namespace Rentz.Intelligence.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HybridPropertySearchController : ControllerBase
{
    private readonly IHybridPropertySearchService _searchService;

    public HybridPropertySearchController(
        IHybridPropertySearchService searchService)
    {
        _searchService = searchService;
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search(
        [FromBody] HybridPropertySearchRequest request,
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

public class HybridPropertySearchRequest
{
    public string Query { get; set; } = string.Empty;

    public int Limit { get; set; } = 10;
}