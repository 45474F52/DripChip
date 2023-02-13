//using DripChip.Models;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;

//namespace DripChip.DataBase.Configuration
//{
//    public sealed class AnimalConfiguration : IEntityTypeConfiguration<Animal>
//    {
//        public void Configure(EntityTypeBuilder<Animal> builder)
//        {
//            builder.HasMany(x => x.AnimalTypes).WithMany(x => x.Animals);
//            builder.HasMany(x => x.VisitedLocations).WithMany(x => x.Animals);

//            builder.HasData(
//                new Animal
//                {
//                    Id = 1,
//                    AnimalTypes = new HashSet<AnimalTypeOnAnimal>()
//                    {
//                        new AnimalTypeOnAnimal() { AnimalId = 1, AnimalTypeId = 3 },
//                        new AnimalTypeOnAnimal() { AnimalId = 1, AnimalTypeId = 5 }
//                    },
//                    Weight = 5,
//                    Length = 1.5f,
//                    Height = .2f,
//                    Gender = Gender.FEMALE,
//                    ChipperId = 1,
//                    ChippingLocationId = 1,
//                    VisitedLocations = new List<VisitedLocationOnAnimal>()
//                    {
//                        new VisitedLocationOnAnimal() { AnimalId = 1, VisitedLocationId = 1 },
//                        new VisitedLocationOnAnimal() { AnimalId = 1, VisitedLocationId = 3 }
//                    }
//                },
//                new Animal
//                {
//                    Id = 2,
//                    AnimalTypes = new List<AnimalTypeOnAnimal>()
//                    {
//                        new AnimalTypeOnAnimal() { AnimalId = 2, AnimalTypeId = 2 },
//                        new AnimalTypeOnAnimal() { AnimalId = 2, AnimalTypeId = 3 }
//                    },
//                    Weight = 5,
//                    Length = 1.3f,
//                    Height = .17f,
//                    Gender = Gender.FEMALE,
//                    ChipperId = 1,
//                    ChippingLocationId = 1,
//                    VisitedLocations = new List<VisitedLocationOnAnimal>()
//                    {
//                        new VisitedLocationOnAnimal() { AnimalId = 2, VisitedLocationId = 3 },
//                        new VisitedLocationOnAnimal() { AnimalId = 2, VisitedLocationId = 5 }
//                    }
//                },
//                new Animal
//                {
//                    Id = 3,
//                    AnimalTypes = new List<AnimalTypeOnAnimal>()
//                    {
//                        new AnimalTypeOnAnimal() { AnimalId = 3, AnimalTypeId = 1 },
//                        new AnimalTypeOnAnimal() { AnimalId = 3, AnimalTypeId = 3 }
//                    },
//                    Weight = 15,
//                    Length = 2.3f,
//                    Height = 1.25f,
//                    Gender = Gender.MALE,
//                    ChipperId = 2,
//                    ChippingLocationId = 2,
//                    VisitedLocations = new List<VisitedLocationOnAnimal>()
//                    {
//                        new VisitedLocationOnAnimal() { AnimalId = 3, VisitedLocationId = 2 },
//                        new VisitedLocationOnAnimal() { AnimalId = 3, VisitedLocationId = 4 }
//                    }
//                }
//                );
//        }
//    }
//}