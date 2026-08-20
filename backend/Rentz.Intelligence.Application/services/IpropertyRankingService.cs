using Rentz.Intelligence.Application.Dtos;

namespace Rentz.Intelligence.Application.Services;

public interface IPropertyRankingService
{
    Task<List<PropertyRankingResponse>> RankPropertiesAsync(
        List<PropertySearchResponse> properties,
        PropertySearchRequest request);
}