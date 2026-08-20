using Microsoft.EntityFrameworkCore;
using Rentz.Intelligence.Application.Dtos;
using Rentz.Intelligence.Application.Services;
using Rentz.Intelligence.Infrastructure.Persistence;

namespace Rentz.Intelligence.Infrastructure.Services;

public class PropertyService : IPropertyService
{
    private readonly RentzDbContext _context;

    public PropertyService(RentzDbContext context)
    {
        _context = context;
    }

    public async Task<List<Domain.Entities.Property>> GetPropertiesAsync()
    {
        return await _context.Properties
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<PropertyDetailsResponse?> GetPropertyAsync(Guid id)
    {
        var property = await _context.Properties
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (property == null)
        {
            return null;
        }

        var amenities = await (
            from pa in _context.PropertyAmenities
            join a in _context.Amenities
                on pa.AmenityId equals a.Id
            where pa.PropertyId == id
            select a.Name
        )
        .AsNoTracking()
        .ToListAsync();

        var images = await _context.PropertyImages
            .AsNoTracking()
            .Where(i => i.PropertyId == id)
            .Select(i => new PropertyImageResponse
            {
                Id = i.Id,
                Url = i.Url,
                AltText = i.AltText
            })
            .ToListAsync();

        var documents = await _context.PropertyDocuments
            .AsNoTracking()
            .Where(d => d.PropertyId == id)
            .Select(d => new PropertyDocumentResponse
            {
                Id = d.Id,
                Name = d.Name,
                DocumentType = d.DocumentType,
                Content = d.Content
            })
            .ToListAsync();

        return new PropertyDetailsResponse
        {
            Id = property.Id,
            Name = property.Name,
            Description = property.Description,
            PropertyType = property.PropertyType,
            City = property.City,
            State = property.State,
            Country = property.Country,
            MonthlyRent = property.MonthlyRent,
            SecurityDeposit = property.SecurityDeposit,
            Bedrooms = property.Bedrooms,
            Bathrooms = property.Bathrooms,
            FurnishingType = property.FurnishingType,
            Amenities = amenities,
            Images = images,
            Documents = documents
        };
    }

    public async Task<List<Domain.Entities.Property>> SearchPropertiesAsync(
        PropertySearchRequest request)
    {
        var query = _context.Properties
            .AsNoTracking()
            .AsQueryable();

        // City filter
        if (!string.IsNullOrWhiteSpace(request.City))
        {
            var city = request.City.Trim().ToLower();

            query = query.Where(p =>
                p.City.ToLower() == city);
        }

        // Minimum rent filter
        if (request.MinRent.HasValue)
        {
            query = query.Where(p =>
                p.MonthlyRent >= request.MinRent.Value);
        }

        // Maximum rent filter
        if (request.MaxRent.HasValue)
        {
            query = query.Where(p =>
                p.MonthlyRent <= request.MaxRent.Value);
        }

        // Minimum bedrooms filter
        if (request.MinBedrooms.HasValue)
        {
            query = query.Where(p =>
                p.Bedrooms >= request.MinBedrooms.Value);
        }

        // Minimum bathrooms filter
        if (request.MinBathrooms.HasValue)
        {
            query = query.Where(p =>
                p.Bathrooms >= request.MinBathrooms.Value);
        }

        // Property type filter
        if (!string.IsNullOrWhiteSpace(request.PropertyType))
        {
            var propertyType = request.PropertyType.Trim();

            query = query.Where(p =>
                p.PropertyType == propertyType);
        }

        return await query
            .OrderBy(p => p.MonthlyRent)
            .ToListAsync();
    }
}