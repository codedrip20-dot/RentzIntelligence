using Rentz.Intelligence.Infrastructure.Services;

namespace Rentz.Intelligence.Tests;

public class QueryUnderstandingServiceTests
{
    private readonly QueryUnderstandingService _service = new();

    [Fact]
    public async Task Should_Extract_City()
    {
        var result = await _service.UnderstandQueryAsync(
            "apartments in Gangtok");

        Assert.Equal("Gangtok", result.City);
    }

    [Fact]
    public async Task Should_Extract_Minimum_Bedrooms()
    {
        var result = await _service.UnderstandQueryAsync(
            "2 bedroom apartment in Gangtok");

        Assert.Equal(2, result.MinBedrooms);
    }

    [Fact]
    public async Task Should_Extract_Minimum_Bathrooms()
    {
        var result = await _service.UnderstandQueryAsync(
            "2 bathroom apartment in Gangtok");

        Assert.Equal(2, result.MinBathrooms);
    }

    [Fact]
    public async Task Should_Extract_Property_Type()
    {
        var result = await _service.UnderstandQueryAsync(
            "apartment in Gangtok");

        Assert.Equal("Apartment", result.PropertyType);
    }

    [Fact]
    public async Task Should_Extract_Max_Rent()
    {
        var result = await _service.UnderstandQueryAsync(
            "apartment in Gangtok under 25k");

        Assert.Equal(25000, result.MaxRent);
    }

    [Fact]
    public async Task Should_Extract_Min_Rent()
    {
        var result = await _service.UnderstandQueryAsync(
            "apartment in Gangtok above 20k");

        Assert.Equal(20000, result.MinRent);
    }

    [Fact]
    public async Task Should_Extract_Rent_Range()
    {
        var result = await _service.UnderstandQueryAsync(
            "apartment in Gangtok between 20k and 22k");

        Assert.Equal(20000, result.MinRent);
        Assert.Equal(22000, result.MaxRent);
    }

    [Fact]
    public async Task Should_Extract_Multiple_Filters()
    {
        var result = await _service.UnderstandQueryAsync(
            "2 bedroom 2 bathroom apartment in Gangtok between 20k and 22k");

        Assert.Equal("Gangtok", result.City);
        Assert.Equal("Apartment", result.PropertyType);
        Assert.Equal(2, result.MinBedrooms);
        Assert.Equal(2, result.MinBathrooms);
        Assert.Equal(20000, result.MinRent);
        Assert.Equal(22000, result.MaxRent);
    }

    [Fact]
    public async Task Should_Return_Empty_Request_For_Empty_Query()
    {
        var result = await _service.UnderstandQueryAsync("");

        Assert.Null(result.City);
        Assert.Null(result.MinRent);
        Assert.Null(result.MaxRent);
        Assert.Null(result.MinBedrooms);
        Assert.Null(result.MinBathrooms);
        Assert.Null(result.PropertyType);
    }
}