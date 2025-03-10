using Spectre.Console;
using CITIES;

namespace PARSING
{
    public class CityFileHandler
    {
        // Чтение городов из CSV
        public List<City> ReadCitiesFromFile(string filePath)
        {
            var cities = new List<City>();

            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var parts = line.Split(',');
                        if (parts.Length >= 4)
                        {
                            var city = new City
                            {
                                Name = parts[0],
                                Country = parts[1],
                                Population = parts.Length > 2 && long.TryParse(parts[2], out var population) ? population : (long?)null,
                                Latitude = double.Parse(parts[3]),
                                Longitude = double.Parse(parts[4])
                            };
                            cities.Add(city);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Ошибка при чтении файла: {ex.Message}[/]");
            }

            return cities;
        }

        // Сохранение городов в файл
        public void SaveCitiesToFile(string filePath, List<City> cities)
        {
            try
            {
                using (var writer = new StreamWriter(filePath))
                {
                    foreach (var city in cities)
                    {
                        writer.WriteLine($"{city.Name},{city.Country},{city.Population},{city.Latitude},{city.Longitude}");
                    }
                }
                AnsiConsole.MarkupLine("[green]Изменения сохранены.[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Ошибка при записи файла: {ex.Message}[/]");
            }
        }
    }
}
