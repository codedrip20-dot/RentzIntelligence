using Microsoft.AspNetCore.Mvc;
using Rentz.Intelligence.Application.Services;

namespace Rentz.Intelligence.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropertiesController : ControllerBase
{
    private readonly IPropertyService _propertyService;
    private readonly IQueryUnderstandingService _queryUnderstandingService;

    public PropertiesController(
        IPropertyService propertyService,
        IQueryUnderstandingService queryUnderstandingService)
    {
        _propertyService = propertyService;
        _queryUnderstandingService = queryUnderstandingService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProperties()
    {
        var properties = await _propertyService.GetPropertiesAsync();

        return Ok(properties);
    }

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

    [HttpGet("search")]
    public async Task<IActionResult> SearchProperties(
        [FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return BadRequest("Search query is required.");
        }

        var searchRequest =
            await _queryUnderstandingService.UnderstandQueryAsync(query);

        var properties =
            await _propertyService.SearchPropertiesAsync(searchRequest);

        return Ok(properties);
    }
}