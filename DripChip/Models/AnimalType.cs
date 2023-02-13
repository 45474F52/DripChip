namespace DripChip.Models
{
    public partial class AnimalType
    {
        public long Id { get; set; }
        public string Type { get; set; } = string.Empty;

        public override string ToString() => $"Id: {Id},\nType: {Type}\n";
    }
}