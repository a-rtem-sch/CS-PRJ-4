using System.Text.Json.Serialization;

namespace CITIES
{
    public class City
    {
        public City()
        {
            Name = string.Empty;
            Country = string.Empty;
        }
        public string Name { get; set; }
        public string Country { get; set; }
        public ulong? Population { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        [JsonIgnore] // Игнорируем поле Marker при сериализации
        public string Marker { get; set; } = "*";
    }
}
