namespace DripChip.Models
{
    public class VisitedLocationOnAnimal
    {
        public long AnimalId { get; set; }
        public long VisitedLocationId { get; set; }
        public virtual ICollection<Animal> Animals { get; set; } = null!;
    }
}