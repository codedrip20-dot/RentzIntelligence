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

    // =========================================================
    // GET ALL PROPERTIES
    // =========================================================

    public async Task<List<Domain.Entities.Property>> GetPropertiesAsync()
    {
        return await _context.Properties
            .AsNoTracking()
            .ToListAsync();
    }

    // =========================================================
    // GET SINGLE PROPERTY
    // =========================================================

    public async Task<PropertyDetailsResponse?> GetPropertyAsync(Guid id)
    {
        var property = await _context.Properties
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (property == null)
        {
            return null;
        }

        // =====================================================
        // AMENITIES
        // =====================================================

        var amenities = await (
            from pa in _context.PropertyAmenities
            join a in _context.Amenities
                on pa.AmenityId equals a.Id
            where pa.PropertyId == id
            select a.Name
        )
        .AsNoTracking()
        .ToListAsync();

        // =====================================================
        // IMAGES
        // =====================================================

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

        // =====================================================
        // DOCUMENTS
        // =====================================================

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

        // =====================================================
        // RESPONSE
        // =====================================================

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

    // =========================================================
    // SEARCH PROPERTIES
    // =========================================================

    public async Task<List<PropertySearchResponse>> SearchPropertiesAsync(
        PropertySearchRequest request)
    {
        var query = _context.Properties
            .AsNoTracking()
            .AsQueryable();

        // =====================================================
        // CITY FILTER
        // =====================================================

        if (!string.IsNullOrWhiteSpace(request.City))
        {
            var city = request.City
                .Trim()
                .ToLower();

            query = query.Where(p =>
                p.City.ToLower() == city);
        }

        // =====================================================
        // MINIMUM RENT FILTER
        // =====================================================

        if (request.MinRent.HasValue)
        {
            query = query.Where(p =>
                p.MonthlyRent >= request.MinRent.Value);
        }

        // =====================================================
        // MAXIMUM RENT FILTER
        // =====================================================

        if (request.MaxRent.HasValue)
        {
            query = query.Where(p =>
                p.MonthlyRent <= request.MaxRent.Value);
        }

        // =====================================================
        // MINIMUM BEDROOMS FILTER
        // =====================================================

        if (request.MinBedrooms.HasValue)
        {
            query = query.Where(p =>
                p.Bedrooms >= request.MinBedrooms.Value);
        }

        // =====================================================
        // MINIMUM BATHROOMS FILTER
        // =====================================================

        if (request.MinBathrooms.HasValue)
        {
            query = query.Where(p =>
                p.Bathrooms >= request.MinBathrooms.Value);
        }

        // =====================================================
        // PROPERTY TYPE FILTER
        // =====================================================

        if (!string.IsNullOrWhiteSpace(request.PropertyType))
        {
            var propertyType = request.PropertyType
                .Trim()
                .ToLower();

            query = query.Where(p =>
                p.PropertyType.ToLower() == propertyType);
        }

        // =====================================================
        // FURNISHING TYPE FILTER
        // =====================================================

        if (!string.IsNullOrWhiteSpace(request.FurnishingType))
        {
            var furnishingType = request.FurnishingType
                .Trim()
                .ToLower();

            query = query.Where(p =>
                p.FurnishingType.ToLower() == furnishingType);
        }

        // =====================================================
        // AMENITY FILTER
        // =====================================================

        if (request.Amenities.Count > 0)
        {
            var requestedAmenities = request.Amenities
                .Select(a => a.Trim().ToLower())
                .Where(a => !string.IsNullOrWhiteSpace(a))
                .ToList();

            foreach (var amenity in requestedAmenities)
            {
                query = query.Where(property =>
                    _context.PropertyAmenities.Any(pa =>
                        pa.PropertyId == property.Id &&
                        _context.Amenities.Any(a =>
                            a.Id == pa.AmenityId &&
                            a.Name.ToLower() == amenity
                        )
                    )
                );
            }
        }

        // =====================================================
        // BUILD SEARCH RESPONSE
        // =====================================================

        return await query
            .OrderBy(p => p.MonthlyRent)
            .Select(p => new PropertySearchResponse
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                PropertyType = p.PropertyType,
                City = p.City,
                State = p.State,
                Country = p.Country,
                MonthlyRent = p.MonthlyRent,
                Bedrooms = p.Bedrooms,
                Bathrooms = p.Bathrooms,
                FurnishingType = p.FurnishingType
            })
            .ToListAsync();
    }
}