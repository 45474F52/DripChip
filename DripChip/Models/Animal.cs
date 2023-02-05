using System.Text.Json.Serialization;

namespace DripChip.Models
{
    public class Animal
    {
        public long Id { get; set; }
        public long[] AnimalTypes { get; set; } = Array.Empty<long>();
        public float Weight { get; set; }
        public float Length { get; set; }
        public float Height { get; set; }
        public string Gender { get; set; } = "OTHER";
        private string _lifeStatus = "DEAD";
        public string LifeStatus
        {
            get => _lifeStatus;
            set
            {
                _lifeStatus = value;
                if (value == "DEATH")
                    DeathDateTime = DateTime.Now.ToString("O");
            }
        }
        public string ChippingDateTime { get; set; }
        public int ChipperId { get; set; }
        public long ChippingLocationId { get; set; }
        public long[] VisitedLocations { get; set; } = Array.Empty<long>();
        public string? DeathDateTime { get; set; } = null;
        [JsonIgnore]
        public AnimalsFilterModel Model { get; set; } = new AnimalsFilterModel();

        public Animal()
        {
            LifeStatus = "ALIVE";
            ChippingDateTime = DateTime.Now.ToString("O");
            DeathDateTime = null;
        }

        public void InitializeFilterModel()
        {
            Model = new()
            {
                Id = this.Id,
                AnimalTypes = this.AnimalTypes,
                Weight = this.Weight,
                Length = this.Length,
                Height = this.Height,
                Gender = this.Gender,
                LifeStatus = this.LifeStatus,
                ChippingDateTime = this.ChippingDateTime,
                ChipperId = this.ChipperId,
                ChippingLocationId = this.ChippingLocationId,
                VisitedLocations = this.VisitedLocations,
                DeathDateTime = this.DeathDateTime
            };
        }

        public override string ToString()
        {
            string animalTypes = string.Empty;
            string visitedLocations = string.Empty;

            foreach (var type in AnimalTypes)
            {
                animalTypes += $"\t{type},\n";
            }

            foreach (var location in VisitedLocations)
            {
                visitedLocations += $"\t{location},\n";
            }

            return
                $"Id: {Id},\n" +
                $"AnimalTypes:{animalTypes}" +
                $"Weight: {Weight},\n" +
                $"Length: {Length},\n" +
                $"Height: {Height},\n" +
                $"Gender: {Gender},\n" +
                $"LifeStatus: {LifeStatus},\n" +
                $"ChippingDateTime: {ChippingDateTime:O},\n" +
                $"ChipperId: {ChipperId},\n" +
                $"ChippingLocationId: {ChippingLocationId},\n" +
                $"VisitedLocations:{visitedLocations}" +
                $"DeathDateTime: {DeathDateTime:O}";
        }
    }

    public sealed class AnimalsFilterModel
    {
        public long Id { get; set; }
        public long[] AnimalTypes { get; set; } = Array.Empty<long>();
        public float Weight { get; set; }
        public float Length { get; set; }
        public float Height { get; set; }
        public string Gender { get; set; } = "OTHER";
        public string LifeStatus { get; set; } = "DEAD";
        public string ChippingDateTime { get; set; }
        public int ChipperId { get; set; }
        public long ChippingLocationId { get; set; }
        public long[] VisitedLocations { get; set; } = Array.Empty<long>();
        public string? DeathDateTime { get; set; } = null;

        public bool Contains(AnimalsFilterModel? fm)
        {
            if (fm is null)
                return false;

            if (ReferenceEquals(this, fm))
                return true;

            bool isContains = true;

            foreach (var fm1props in this.GetType().GetProperties())
            {
                if (fm1props.Name == nameof(ChipperId)
                    || fm1props.Name == nameof(ChippingLocationId)
                    || fm1props.Name == nameof(LifeStatus)
                    || fm1props.Name == nameof(Gender))
                {
                    foreach (var fm2props in fm.GetType().GetProperties())
                    {
                        if (fm1props.Name == fm2props.Name)
                        {
                            if (fm1props.GetValue(this)!.ToString()!.ToLower().Contains(fm2props.GetValue(fm)!.ToString()!.ToLower()))
                            {
                                isContains = true;
                                break;
                            }
                            else
                                return false;
                        }
                        else
                            continue;
                    }
                }
                else
                    continue;
            }

            return isContains;
        }

        public override bool Equals(object? obj)
        {
            if (obj is AnimalsFilterModel filterModel)
                return this == filterModel;
            else
                return false;
        }

        public static bool operator ==(AnimalsFilterModel? fm1, AnimalsFilterModel? fm2)
        {
            if (fm1 is null || fm2 is null)
                return false;

            if (ReferenceEquals(fm1, fm2))
                return true;

            bool isEqulas = true;

            foreach (var fm1props in fm1.GetType().GetProperties())
            {
                foreach (var fm2props in fm2.GetType().GetProperties())
                {
                    if (fm1props.Name == fm2props.Name && fm1props.Name != nameof(Id))
                    {
                        if (fm1props.GetValue(fm1)!.Equals(fm2props.GetValue(fm2)))
                        {
                            isEqulas = true;
                            break;
                        }
                        else
                            return false;
                    }
                    else
                        continue;
                }
            }

            return isEqulas;
        }

        public static bool operator !=(AnimalsFilterModel? fm1, AnimalsFilterModel? fm2)
        {
            if (fm1 is null || fm2 is null)
                return false;

            if (ReferenceEquals(fm1, fm2))
                return false;

            bool isNotEqulas = false;

            foreach (var fm1props in fm1.GetType().GetProperties())
            {
                foreach (var fm2props in fm2.GetType().GetProperties())
                {
                    if (fm1props.Name == fm2props.Name)
                    {
                        if (!fm1props.GetValue(fm1)!.Equals(fm2props.GetValue(fm2)))
                        {
                            isNotEqulas = true;
                            break;
                        }
                        else
                            return false;
                    }
                    else
                        continue;
                }
            }

            return isNotEqulas;
        }

        public override int GetHashCode() => GetHashCode();
    }
}