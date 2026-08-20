namespace Rentz.Intelligence.Domain.Entities;

public class Amenity
{
    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    private Amenity()
    {
    }

    public static Amenity Create(Guid id, string name)
    {
        return new Amenity
        {
            Id = id,
            Name = name
        };
    }
}