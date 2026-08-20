using Rentz.Intelligence.Application.Dtos;
using Rentz.Intelligence.Domain.Entities;

namespace Rentz.Intelligence.Application.Services;

public interface IPropertyService
{
    Task<List<Property>> GetPropertiesAsync();

    Task<PropertyDetailsResponse?> GetPropertyAsync(Guid id);

    Task<List<PropertySearchResponse>> SearchPropertiesAsync(
        PropertySearchRequest request);
}