namespace Rentz.Intelligence.Domain.Entities;

public class PropertyDocument
{
    public Guid Id { get; private set; }

    public Guid PropertyId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string DocumentType { get; private set; } = string.Empty;

    public string Content { get; private set; } = string.Empty;

    public static PropertyDocument Create(
        Guid id,
        Guid propertyId,
        string name,
        string documentType,
        string content)
    {
        return new PropertyDocument
        {
            Id = id,
            PropertyId = propertyId,
            Name = name,
            DocumentType = documentType,
            Content = content
        };
    }

    private PropertyDocument()
    {
    }
}