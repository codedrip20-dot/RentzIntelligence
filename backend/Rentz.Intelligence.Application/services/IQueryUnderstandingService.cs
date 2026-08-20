using Rentz.Intelligence.Application.Dtos;

namespace Rentz.Intelligence.Application.Services;

public interface IQueryUnderstandingService
{
    Task<PropertySearchRequest> UnderstandQueryAsync(
        string query);
}