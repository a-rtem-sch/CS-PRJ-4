using GEOCODING;
using Spectre.Console;
using System.Globalization;

namespace CITIES
{
    /// <summary>
    /// Класс, предоставляющий методы для работы с коллекцией городов
    /// </summary>
    public class CityManager(CityCollection cityCollection)
    {
        private readonly CityCollection _cityCollection = cityCollection;

        /// <summary>
        /// Сразу схлопываем асинхронную функцию
        /// </summary>
        public void AddCity()
        {
            AddCityAsync().GetAwaiter().GetResult();
        }
        /// <summary>
        /// Асинхронная функция для получения геоинформации
        /// </summary>
        /// <returns></returns>
        public async Task AddCityAsync()
        {
            string name = AnsiConsole.Ask<string>("Введите название города:");
            string country = AnsiConsole.Ask<string>("Введите страну:");

            ulong? population = GetPopulationInput(null);


            //запрос
            (double Latitude, double Longitude)? coordinates = await GeocodingService.GeocodeAsync($"{name}, {country}");

            double latitude, longitude;

            if (coordinates.HasValue)
            {
                AnsiConsole.MarkupLine($"[green]Найдены координаты: {coordinates.Value.Latitude}, {coordinates.Value.Longitude}[/]");
                bool confirm = AnsiConsole.Confirm("Подтвердить эти координаты?");

                if (confirm)
                {
                    latitude = coordinates.Value.Latitude;
                    longitude = coordinates.Value.Longitude;
                }
                else
                {
                    latitude = AnsiConsole.Ask<double>("Введите широту вручную:");
                    longitude = AnsiConsole.Ask<double>("Введите долготу вручную:");
                }
            }
            else
            {
                AnsiConsole.MarkupLine("[yellow]Координаты не найдены. Введите их вручную.[/]");
                latitude = AnsiConsole.Prompt(
                                                new TextPrompt<double>("Введите широту:")
                                                  .Validate((n) => n switch
                                                  {
                                                      < -90 => ValidationResult.Error("[red]Широта не может быть меньше -90.[/]"),
                                                      > 90 => ValidationResult.Error("[red]Широта не может быть больше 90.[/]"),
                                                      _ => ValidationResult.Success()
                                                  })
);
                longitude = AnsiConsole.Prompt(
                                                new TextPrompt<double>("Введите долготу: ")
                                                  .Validate((n) => n switch
                                                  {
                                                      < -180 => ValidationResult.Error("[red]Долгота не может быть меньше -180.[/]"),
                                                      > 180 => ValidationResult.Error("[red]Долгота не может быть больше 180.[/]"),
                                                      _ => ValidationResult.Success()
                                                  })
);
            }

            City city = new()
            {
                Name = name,
                Country = country,
                Population = population,
                Latitude = latitude,
                Longitude = longitude
            };

            _cityCollection.AddCity(city);
            AnsiConsole.MarkupLine("[green]Город успешно добавлен. Нажмите любую клавишу для продолжения:[/]");
            _ = Console.ReadKey(intercept: true);
            Console.Clear();
            Console.WriteLine("\x1b[3J");
        }

        /// <summary>
        /// Служебный метод для получения населения (нетривиальная обработка, сделал отдельно чтобы не грузить код)
        /// </summary>
        /// <param name="pop">Плейсхолдер, null для первого назначения</param>
        /// <returns></returns>
        private ulong? GetPopulationInput(ulong? pop)
        {
            while (true)
            {
                string populationInput = pop == null
                    ? AnsiConsole.Prompt(
                        new TextPrompt<string>("Введите население (опционально, нажмите Enter чтобы пропустить):")
                            .AllowEmpty()
                    )
                    : AnsiConsole.Prompt(
                        new TextPrompt<string>("Введите население (опционально, нажмите Enter чтобы пропустить):")
                            .DefaultValue(pop?.ToString() ?? "")
                            .AllowEmpty()
                    );
                if (string.IsNullOrEmpty(populationInput))
                {
                    return null;
                }

                if (ulong.TryParse(populationInput, NumberStyles.Any, CultureInfo.InvariantCulture, out ulong population))
                {
                    return population;
                }

                AnsiConsole.MarkupLine("[red]Некорректный формат числа. Пожалуйста, введите число.[/]");
            }
        }


        /// <summary>
        /// Изменение города
        /// </summary>
        /// <param name="cities">города</param>
        public void EditCity(CityCollection cities)
        {
            if (cities.Cities.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]Список городов пуст.[/]");
                return;
            }

            List<string> cityNames = _cityCollection.Cities.Select(c => c.Name).ToList();
            cityNames.Add("Назад");

            string selectedCityName = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Выберите город:")
                    .AddChoices(cityNames));

            if (selectedCityName == "Назад")
            {
                return;
            }

            City city = _cityCollection.GetCityByName(selectedCityName);

            city.Name = AnsiConsole.Prompt(
                new TextPrompt<string>("Введите новое название города:")
                    .DefaultValue(city.Name));
            city.Country = AnsiConsole.Prompt(
                new TextPrompt<string>("Введите новую страну:")
                    .DefaultValue(city.Country));
            city.Population = GetPopulationInput(city.Population);
            city.Latitude = AnsiConsole.Prompt(
                new TextPrompt<double>("Введите новую широту:")
                    .DefaultValue(city.Latitude));
            city.Longitude = AnsiConsole.Prompt(
                new TextPrompt<double>("Введите новую долготу:")
                    .DefaultValue(city.Longitude));

            AnsiConsole.MarkupLine("[green]Информация о городе успешно обновлена. Нажмите любую клавишу для продолжения:[/]");
            _ = Console.ReadKey(intercept: true);
            Console.Clear();
            Console.WriteLine("\x1b[3J");
        }

        // Удаление города
        public void DeleteCity()
        {
            List<string> cityNames = _cityCollection.Cities.Select(c => c.Name).ToList();
            cityNames.Add("Назад");
            string selectedCityName = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Выберите город:")
                    .AddChoices(cityNames));

            if (selectedCityName == "Назад")
            {
                return;
            }


            _cityCollection.DeleteCity(selectedCityName);
            AnsiConsole.MarkupLine("[green]Город успешно удален. Нажмите любую клавишу для продолжения:[/]");
            _ = Console.ReadKey(intercept: true);
            Console.Clear();
            Console.WriteLine("\x1b[3J");
        }
    }
}
