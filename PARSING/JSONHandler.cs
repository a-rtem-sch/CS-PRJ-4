﻿using CITIES;
using System.Text.Json;

namespace PARSING
{
    /// <summary>
    /// Парсинг JSON
    /// </summary>
    public class JSONHandler
    {
        /// <summary>
        /// Получение городов
        /// </summary>
        /// <param name="filePath">путь к файлу</param>
        /// <returns> кортеж из городов и ошибок</returns>
        public static (List<City> Cities, List<BadRecord> BadRecords) ImportCitiesFromJson(string filePath)
        {
            List<BadRecord> badRecords = [];
            string absolutePath = Path.GetFullPath(filePath);
            // Опции парсинга
            JsonSerializerOptions options = new ()
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals
            };

            string json = File.ReadAllText(absolutePath);

            List<JsonElement>? jsonElements;
            try
            {
                jsonElements = JsonSerializer.Deserialize<List<JsonElement>>(json, options);
            }
            catch (JsonException)
            {
                throw;
            }

            List<City> cities = [];
            foreach (JsonElement jsonElement in jsonElements)
            {
                try
                {
                    City? city = JsonSerializer.Deserialize<City>(jsonElement.GetRawText(), options);
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
        /// <summary>
        /// Запись городов в JSON
        /// </summary>
        /// <param name="cityCollection"></param>
        /// <param name="filePath"></param>
        public static void ExportCitiesToJson(CityCollection cityCollection, string filePath)
        {
            List<City> cities = cityCollection.Cities;
            JsonSerializerOptions options = new ()
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals
            };

            string json = JsonSerializer.Serialize(cities, options);
            File.WriteAllText(filePath, json);
        }
    }
}
