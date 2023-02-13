using DripChip.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DripChip.DataBase.Configuration
{
    public sealed class AnimalTypeConfiguration : IEntityTypeConfiguration<AnimalType>
    {
        public void Configure(EntityTypeBuilder<AnimalType> builder)
        {
            builder.HasData(
                new AnimalType { Id = 1, Type = "Тип_1" },
                new AnimalType { Id = 2, Type = "Тип_2" },
                new AnimalType { Id = 3, Type = "Тип_3" },
                new AnimalType { Id = 4, Type = "Тип_4" },
                new AnimalType { Id = 5, Type = "Тип_5" }
                );
        }
    }
}