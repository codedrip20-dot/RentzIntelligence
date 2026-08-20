namespace Rentz.Intelligence.Domain.Entities;

public class PropertyImage
{
    public Guid Id { get; private set; }

    public Guid PropertyId { get; private set; }

    public string Url { get; private set; } = string.Empty;

    public string AltText { get; private set; } = string.Empty;

    public static PropertyImage Create(
    Guid id,
    Guid propertyId,
    string url,
    string altText)
{
    return new PropertyImage
    {
        Id = id,
        PropertyId = propertyId,
        Url = url,
        AltText = altText
    };
}

    private PropertyImage()
    {
    }
}
