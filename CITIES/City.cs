using System.Text.Json.Serialization;

namespace CITIES
{
    /// <summary>
    /// Класс, представляющий город. Публичные данные для удобства изменения
    /// </summary>
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

        // не пишем в JSON
        [JsonIgnore]
        public string Marker { get; set; } = "*";
    }
}
