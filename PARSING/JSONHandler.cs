using CITIES;
using System.Text.Json;

namespace PARSING
{
    public class JSONHandler
    {
        public static (List<City> Cities, List<BadRecord> BadRecords) ImportCitiesFromJson(string filePath)
        {
            List<BadRecord> badRecords = new List<BadRecord>();
            string absolutePath = Path.GetFullPath(filePath);
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals
            };

            var json = File.ReadAllText(absolutePath);

            List<JsonElement>? jsonElements;
            try
            {
                jsonElements = JsonSerializer.Deserialize<List<JsonElement>>(json, options);
            }
            catch (JsonException)
            {
                throw;
            }

            var cities = new List<City>();
            foreach (var jsonElement in jsonElements)
            {
                try
                {
                    var city = JsonSerializer.Deserialize<City>(jsonElement.GetRawText(), options);
                    if (city != null)
                    {
                        cities.Add(city);
                    }
                }
                catch (JsonException ex)
                {
                    badRecords.Add(new BadRecord
                    {
                        RawRecord = ex.Message ?? "N/A",
                    });
                    continue;
                }
            }

            return (cities, badRecords);
        }
        public static void ExportCitiesToJson(List<City> cities, string filePath)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals
            };

            var json = JsonSerializer.Serialize(cities, options);
            File.WriteAllText(filePath, json);
        }
    }
}
