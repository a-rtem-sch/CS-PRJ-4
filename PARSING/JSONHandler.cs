using CITIES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PARSING
{
    public class JSONHandler
    {
        public static List<City> ImportCitiesFromJson(string filePath)
        {
            string absolutePath = Path.GetFullPath(filePath);
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals
            };
            var json = File.ReadAllText(absolutePath);
            return JsonSerializer.Deserialize<List<City>>(json, options);
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
