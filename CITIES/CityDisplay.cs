using GEOCODING;
using Spectre.Console;
using System.Diagnostics.Metrics;
using System.Xml.Linq;

namespace CITIES
{
    public class CityDisplay
    {
        private readonly CityCollection _cityCollection;

        public CityDisplay(CityCollection cityCollection)
        {
            _cityCollection = cityCollection;
        }

        public void DisplayCitiesTable()
        {
            var table = new Table();
            table.AddColumn("Название");
            table.AddColumn("Страна");
            table.AddColumn("Население");
            table.AddColumn("Широта");
            table.AddColumn("Долгота");

            foreach (var city in _cityCollection.Cities)
            {
                table.AddRow(
                    city.Name,
                    city.Country,
                    city.Population?.ToString() ?? "N/A",
                    city.Latitude.ToString(),
                    city.Longitude.ToString()
                );
            }

            AnsiConsole.Write(table);
            AnsiConsole.MarkupLine("[green]Нажмите любую клавишу для продолжения:[/]");
            Console.ReadKey(intercept: true);
            Console.Clear();
        }

        public void SelectAndDisplayCity(CityCollection cityCollection)
        {
            var cities = cityCollection.Cities;

            if (cities.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]Список городов пуст.[/]");
                return;
            }
            var cityNames = cities.Select(c => c.Name).ToList();
            cityNames.Add("Назад");
            string selectedCityName = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Выберите город:")
                    .AddChoices(cityNames));

            if (selectedCityName == "Назад")
            {
                return;
            }

            var selectedCity = cities.FirstOrDefault(c => c.Name == selectedCityName);

            if (selectedCity != null)
            {
                DisplayCityInfo(selectedCity);
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Город не найден.[/]");
            }
        }
        public void DisplayCityInfo(City city)
        {
            var data = GeocodingFillers.ReverseGeocode(city.Latitude, city.Longitude);
            var weather = WeatherResponse.GetWeather(city.Latitude, city.Longitude);

            AnsiConsole.MarkupLine("[bold underline cyan]Информация о городе:[/]");
            AnsiConsole.MarkupLine($"[bold]Название:[/] {city.Name}");
            AnsiConsole.MarkupLine($"[bold]Страна:[/] {city.Country}");
            AnsiConsole.MarkupLine($"[bold]Население:[/] {(city.Population.HasValue ? city.Population.Value.ToString("N0") : "N/A")}");
            AnsiConsole.MarkupLine($"[bold]Координаты:[/] Широта: {city.Latitude}, Долгота: {city.Longitude}");

            // Вывод дополнительной информации из JSON
            if (data != null)
            {
                AnsiConsole.MarkupLine("[bold underline cyan]Дополнительная информация (обратное геокодирование по широте и долготе):[/]");
                AnsiConsole.MarkupLine($"[bold]Полное название:[/] {data.Display_Name}");
                AnsiConsole.MarkupLine($"[bold]Тип объекта:[/] {data.Name}");
                AnsiConsole.MarkupLine($"[bold]Адрес:[/] {data.Address?.Road} {data.Address?.House_Number}, {data.Address?.City}, {data.Address?.Country}");
                AnsiConsole.MarkupLine($"[bold]Почтовый индекс:[/] {data.Address?.Postcode}");
                AnsiConsole.MarkupLine($"[bold]Границы:[/] Широта: {data.Boundingbox?[0]} - {data.Boundingbox?[1]}, Долгота: {data.Boundingbox?[2]} - {data.Boundingbox?[3]}");
            }

            if (weather != null)
            {
                AnsiConsole.MarkupLine("[bold underline cyan]Текущая погода:[/]");
                AnsiConsole.MarkupLine($"[bold]Температура:[/] {WeatherResponse.KelvinToCelsius(weather.Main.Temp):F1}°C");
                AnsiConsole.MarkupLine($"[bold]Погода:[/] {weather.Weather[0].Description}");
                AnsiConsole.MarkupLine($"[bold]Влажность:[/] {weather.Main.Humidity}%");
                AnsiConsole.MarkupLine($"[bold]Скорость ветра:[/] {weather.Wind.Speed} м/с");
                AnsiConsole.MarkupLine($"[bold]Облачность:[/] {weather.Clouds.All}%");
            }

            AnsiConsole.MarkupLine("[green]Нажмите любую клавишу для продолжения:[/]");
            Console.ReadKey(intercept: true);
            Console.Clear();
        }
    }
}
