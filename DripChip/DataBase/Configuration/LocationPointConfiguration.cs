using DripChip.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DripChip.DataBase.Configuration
{
    public sealed class LocationPointConfiguration : IEntityTypeConfiguration<LocationPoint>
    {
        public void Configure(EntityTypeBuilder<LocationPoint> builder)
        {
            builder.HasData(
                new LocationPoint { Id = 1, Latitude = 80.45, Longitude = 33.15 },
                new LocationPoint { Id = 2, Latitude = 10.27, Longitude = -20.67 },
                new LocationPoint { Id = 3, Latitude = 45.45, Longitude = -54.98 },
                new LocationPoint { Id = 4, Latitude = -33.71, Longitude = -150.04 },
                new LocationPoint { Id = 5, Latitude = -76.23, Longitude = 100.04 }
                );
        }
    }
}