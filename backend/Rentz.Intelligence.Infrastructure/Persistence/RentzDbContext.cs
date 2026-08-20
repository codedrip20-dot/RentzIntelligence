using Microsoft.EntityFrameworkCore;
using Rentz.Intelligence.Domain.Entities;

namespace Rentz.Intelligence.Infrastructure.Persistence;

public class RentzDbContext : DbContext
{
    public RentzDbContext(DbContextOptions<RentzDbContext> options)
        : base(options)
    {
    }

    public DbSet<Property> Properties => Set<Property>();

    public DbSet<Amenity> Amenities => Set<Amenity>();

    public DbSet<PropertyAmenity> PropertyAmenities => Set<PropertyAmenity>();

    public DbSet<PropertyImage> PropertyImages => Set<PropertyImage>();

    public DbSet<PropertyDocument> PropertyDocuments => Set<PropertyDocument>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(RentzDbContext).Assembly);
    }
}