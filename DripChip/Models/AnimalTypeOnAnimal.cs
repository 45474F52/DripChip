namespace DripChip.Models
{
    public class AnimalTypeOnAnimal
    {
        public long AnimalId { get; set; }
        public long AnimalTypeId { get; set; }
        public virtual ICollection<Animal> Animals { get; set; } = null!;
    }
}