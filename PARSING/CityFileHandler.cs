using Spectre.Console;
using CITIES;
using System.Globalization;


//ИСПОЛЬЗОВАЛОСЬ ДО БИБЛИОТЕКИ. ОСТАВИЛ КАК АРХИВ


//namespace PARSING
//{
//    public class CityFileHandler
//    {
//        // Чтение городов из CSV
//        public (List<City>, List<string>) ReadCitiesFromFile(string filePath)
//        {
//            List<City> cities = new List<City>();
//            List<string> errors = new List<string>();

//            try
//            {

//                string absolutePath = Path.GetFullPath(filePath);
//                using (var reader = new StreamReader(absolutePath))
//                {
//                    string line;
//                    while ((line = reader.ReadLine()) != null)
//                    {

//                        var parts = line.Split(',');

//                        if (parts.Length >= 4)
//                        {
//                            if (double.TryParse(parts[parts.Length - 2].Trim().Replace('.', ','), out var latitude) &&
//                                double.TryParse(parts[parts.Length - 1].Trim().Replace('.', ','), out var longitude))
//                            {
//                                var city = new City
//                                {
//                                    Name = parts[0].Trim(),
//                                    Country = parts[1].Trim(),
//                                    Latitude = latitude,
//                                    Longitude = longitude
//                                };

//                                if (parts.Length > 4 && ulong.TryParse(parts[2].Trim(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var population))
//                                {
//                                    city.Population = population;
//                                }

//                                cities.Add(city);
//                            }
//                            else
//                            {
//                                //AnsiConsole.MarkupLine($"[yellow]Пропущена строка \"{line}\": некорректные координаты.[/]");
//                                errors.Add(line + "##неверный формат координат");
//                            }
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                AnsiConsole.MarkupLine($"[red]Ошибка при чтении файла: {ex.Message}[/]");
//            }
//            if (errors.Count > 0)
//            {
//                AnsiConsole.MarkupLine($"[yellow]Пропущены одна или более строк данных.\nПодробнее в разделе \"Просмотреть пропущенные строки\"[/]");
//            }
//            return (cities, errors);
//        }

//        // Сохранение городов в файл
//        public void SaveCitiesToFile(string filePath, List<City> cities)
//        {
//            try
//            {
//                string absolutePath = Path.GetFullPath(filePath);
//                using (var writer = new StreamWriter(absolutePath))
//                {
//                    foreach (var city in cities)
//                    {
//                        writer.WriteLine(
//                        $"{city.Name}," +
//                        $"{city.Country}," +
//                        $"{(city.Population.HasValue ? city.Population.Value.ToString() : "")}," +
//                        $"{city.Latitude.ToString(CultureInfo.InvariantCulture)}," +
//                        $"{city.Longitude.ToString(CultureInfo.InvariantCulture)}"
//                );
//                    }
//                }
//                AnsiConsole.MarkupLine("[green]Изменения сохранены.[/]");
//            }
//            catch (Exception ex)
//            {
//                AnsiConsole.MarkupLine($"[red]Ошибка при записи файла: {ex.Message}[/]");
//            }
//        }
//    }
//}
