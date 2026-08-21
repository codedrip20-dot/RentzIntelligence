using Microsoft.AspNetCore.Mvc;
using Rentz.Intelligence.Application.Services;

namespace Rentz.Intelligence.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropertiesController : ControllerBase
{
    private readonly IPropertyService _propertyService;
    private readonly IQueryUnderstandingService _queryUnderstandingService;
    private readonly IPropertyRankingService _propertyRankingService;

    public PropertiesController(
        IPropertyService propertyService,
        IQueryUnderstandingService queryUnderstandingService,
        IPropertyRankingService propertyRankingService)
    {
        _propertyService = propertyService;
        _queryUnderstandingService = queryUnderstandingService;
        _propertyRankingService = propertyRankingService;
    }

    // =========================================================
    // GET ALL PROPERTIES
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> GetProperties()
    {
        var properties = await _propertyService.GetPropertiesAsync();

        return Ok(properties);
    }

    // =========================================================
    // GET SINGLE PROPERTY
    // =========================================================

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProperty(Guid id)
    {
        var property = await _propertyService.GetPropertyAsync(id);

        if (property == null)
        {
            return NotFound();
        }

        return Ok(property);
    }

    // =========================================================
    // SEARCH + RANK PROPERTIES
    // =========================================================

    [HttpGet("search")]
    public async Task<IActionResult> SearchProperties(
        [FromQuery] string query)
    {
        // =====================================================
        // VALIDATE QUERY
        // =====================================================

        if (string.IsNullOrWhiteSpace(query))
        {
            return BadRequest("Search query is required.");
        }

        // =====================================================
        // STEP 1 — UNDERSTAND NATURAL LANGUAGE
        // =====================================================

        var searchRequest =
            await _queryUnderstandingService
                .UnderstandQueryAsync(query);

        // =====================================================
        // STEP 2 — FILTER PROPERTIES
        // =====================================================

        var properties =
            await _propertyService
                .SearchPropertiesAsync(searchRequest);

        // =====================================================
        // STEP 3 — RANK MATCHING PROPERTIES
        // =====================================================

        var rankedProperties =
            await _propertyRankingService
                .RankPropertiesAsync(
                    properties,
                    searchRequest);

        // =====================================================
        // STEP 4 — RETURN SEARCH RESULTS + AI SOURCE
        // =====================================================

        return Ok(new
        {
            query,

            understanding = new
            {
                source = searchRequest.Source.ToString()
            },

            results = rankedProperties
        });
    }
}