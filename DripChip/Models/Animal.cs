using System.Text.Json.Serialization;

namespace DripChip.Models
{
    public partial class Animal
    {
        /// <summary>
        /// Идентификатор животного
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// Идентификаторы типов животного
        /// </summary>
        public ICollection<AnimalTypeOnAnimal> AnimalTypes { get; set; }
        /// <summary>
        /// Масса (кг)
        /// </summary>
        public float Weight { get; set; }
        /// <summary>
        /// Длина (м)
        /// </summary>
        public float Length { get; set; }
        /// <summary>
        /// Высота (м)
        /// </summary>
        public float Height { get; set; }
        /// <summary>
        /// Гендерный признак животного
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Gender Gender { get; set; }
        private LifeStatus _lifeStatus;
        /// <summary>
        /// Жизненный статус животного
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public LifeStatus LifeStatus
        {
            get => _lifeStatus;
            set
            {
                _lifeStatus = value;
                if (value == LifeStatus.DEAD)
                    DeathDateTime = DateTime.Now.ToString("O");
            }
        }
        /// <summary>
        /// Дата и время чипирования (ISO-8601)
        /// </summary>
        public string ChippingDateTime { get; set; }
        /// <summary>
        /// Идентификатор аккаунта чиппера
        /// </summary>
        public int ChipperId { get; set; }
        /// <summary>
        /// Идентификатор точки локации животных
        /// </summary>
        public long ChippingLocationId { get; set; }
        /// <summary>
        /// Идентификаторы объектов с информацией о посещённых точках локаций
        /// </summary>
        public ICollection<VisitedLocationOnAnimal> VisitedLocations { get; set; }
        /// <summary>
        /// Дата и время смерти животного (ISO-8601)
        /// </summary>
        public string? DeathDateTime { get; set; } = null;

        public Animal()
        {
            AnimalTypes = new HashSet<AnimalTypeOnAnimal>();
            VisitedLocations = new HashSet<VisitedLocationOnAnimal>();
            LifeStatus = LifeStatus.ALIVE;
            ChippingDateTime = DateTime.Now.ToString("O");
            DeathDateTime = null;
        }

        public void UpdateProperties(Animal other)
        {
            AnimalTypes = other.AnimalTypes;
            Weight = other.Weight;
            Length = other.Length;
            Height = other.Height;
            Gender = other.Gender;
            LifeStatus = other.LifeStatus;
            ChippingDateTime = other.ChippingDateTime;
            ChipperId = other.ChipperId;
            ChippingLocationId = other.ChippingLocationId;
            VisitedLocations = other.VisitedLocations;
            DeathDateTime = other.DeathDateTime;
        }

        public bool Contains(Animal? other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            bool isContains = true;

            foreach (var myProp in this.GetType().GetProperties())
            {
                if (myProp.Name == nameof(ChipperId)
                    || myProp.Name == nameof(ChippingLocationId)
                    || myProp.Name == nameof(LifeStatus)
                    || myProp.Name == nameof(Gender))
                {
                    foreach (var otherProp in other.GetType().GetProperties())
                    {
                        if (myProp.Name == otherProp.Name)
                        {
                            if (myProp.GetValue(this)!.ToString()!.ToLower().Contains(otherProp.GetValue(other)!.ToString()!.ToLower()))
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

        public override string ToString()
        {
            string animalTypes = string.Empty;
            string visitedLocations = string.Empty;

            foreach (var type in AnimalTypes)
                animalTypes += $"\t{type.AnimalTypeId},\n";

            foreach (var location in VisitedLocations)
                visitedLocations += $"\t{location.VisitedLocationId},\n";

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
}