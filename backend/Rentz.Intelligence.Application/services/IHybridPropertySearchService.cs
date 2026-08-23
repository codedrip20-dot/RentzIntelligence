using Rentz.Intelligence.Application.Dtos;

namespace Rentz.Intelligence.Application.Services;

public interface IHybridPropertySearchService
{
    Task<List<HybridPropertySearchResponse>> SearchAsync(
        string query,
        int limit = 10,
        CancellationToken cancellationToken = default);
}