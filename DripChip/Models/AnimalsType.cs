namespace DripChip.Models
{
    public class AnimalsType
    {
        public long Id { get; set; }
        public string Type { get; set; } = string.Empty;

        public override string ToString() => $"Id: {Id},\nType: {Type}\n";
    }
}