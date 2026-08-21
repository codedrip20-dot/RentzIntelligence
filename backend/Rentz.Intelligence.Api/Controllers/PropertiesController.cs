using Microsoft.AspNetCore.Mvc;
using Rentz.Intelligence.Application.Services;

namespace Rentz.Intelligence.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropertiesController : ControllerBase
{
    private readonly IPropertyService _propertyService;
    private readonly IHybridPropertySearchService _hybridSearchService;

    public PropertiesController(
        IPropertyService propertyService,
        IHybridPropertySearchService hybridSearchService)
    {
        _propertyService = propertyService;
        _hybridSearchService = hybridSearchService;
    }

    // =========================================================
    // GET ALL PROPERTIES
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> GetProperties()
    {
        var properties =
            await _propertyService.GetPropertiesAsync();

        return Ok(properties);
    }

    // =========================================================
    // GET SINGLE PROPERTY
    // =========================================================

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProperty(Guid id)
    {
        var property =
            await _propertyService.GetPropertyAsync(id);

        if (property == null)
        {
            return NotFound();
        }

        return Ok(property);
    }

    // =========================================================
    // HYBRID PROPERTY SEARCH
    // =========================================================

    [HttpGet("search")]
    public async Task<IActionResult> SearchProperties(
        [FromQuery] string query,
        CancellationToken cancellationToken)
    {
        // =====================================================
        // VALIDATE QUERY
        // =====================================================

        if (string.IsNullOrWhiteSpace(query))
        {
            return BadRequest("Search query is required.");
        }

        // =====================================================
        // HYBRID SEARCH
        // =====================================================

        var results =
            await _hybridSearchService.SearchAsync(
                query,
                10,
                cancellationToken);

        // =====================================================
        // RETURN RESULTS
        // =====================================================

        return Ok(new
        {
            query,
            results
        });
    }
}