namespace DripChip.Models
{
    public partial class AnimalVisitedLocations
    {
        public long Id { get; set; }
        public DateTime DateTimeOfVisistLocationPoint { get; set; }
        public long LocationPointId { get; set; }
    }
}