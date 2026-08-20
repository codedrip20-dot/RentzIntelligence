using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rentz.Intelligence.Domain.Entities;

namespace Rentz.Intelligence.Infrastructure.Persistence.Configurations;

public class PropertyAmenityConfiguration : IEntityTypeConfiguration<PropertyAmenity>
{
    public void Configure(EntityTypeBuilder<PropertyAmenity> builder)
    {
        builder.HasKey(pa => new
        {
            pa.PropertyId,
            pa.AmenityId
        });
    }
}