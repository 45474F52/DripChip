namespace DripChip.Models
{
    public class LocationPoint
    {
        public long Id { get; set; }
        /// <summary>
        /// Географическая широта в градусах
        /// </summary>
        public double Latitude { get; set; }
        /// <summary>
        /// Географическая долгота в градусах
        /// </summary>
        public double Longitude { get; set; }
    }
}