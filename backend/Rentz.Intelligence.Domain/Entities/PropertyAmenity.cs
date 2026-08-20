namespace Rentz.Intelligence.Domain.Entities;

public class PropertyAmenity
{
    public Guid PropertyId { get; private set; }

    public Guid AmenityId { get; private set; }

    private PropertyAmenity()
    {
    }

    public static PropertyAmenity Create(
        Guid propertyId,
        Guid amenityId)
    {
        return new PropertyAmenity
        {
            PropertyId = propertyId,
            AmenityId = amenityId
        };
    }
}