namespace Rentz.Intelligence.Application.Dtos;

public class PropertyRankingResponse
{
    public Guid PropertyId { get; set; }

    public double Score { get; set; }

    public string Reason { get; set; } = string.Empty;
}