using DripChip.Models;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace DripChip.DataBase
{
    public partial class DataContext : DbContext
    {
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Animal> Animals { get; set; }
        public virtual DbSet<AnimalType> AnimalsTypes { get; set; }  
        public virtual DbSet<AnimalVisitedLocations> AnimalVisitedLocations { get; set; }
        public virtual DbSet<LocationPoint> LocationPoints { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options) => Database.EnsureCreated();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AnimalTypeOnAnimal>().HasKey(x => new { x.AnimalId, x.AnimalTypeId });
            modelBuilder.Entity<VisitedLocationOnAnimal>().HasKey(x => new { x.AnimalId, x.VisitedLocationId });

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}