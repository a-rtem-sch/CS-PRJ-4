﻿using GEOCODING;
using Spectre.Console;

namespace CITIES
{
    /// <summary>
    /// Отображение городов в различном виде
    /// </summary>
    public class CityDisplay(CityCollection cityCollection)
    {
        private readonly CityCollection _cityCollection = cityCollection;

        /// <summary>
        /// Вывод в  виде таблички
        /// </summary>
        public void DisplayCitiesTable()
        {
            Table table = new();
            _ = table.AddColumn("Название");
            _ = table.AddColumn("Страна");
            _ = table.AddColumn("Население");
            _ = table.AddColumn("Широта");
            _ = table.AddColumn("Долгота");

            foreach (City city in _cityCollection.Cities)
            {
                _ = table.AddRow(
                    city.Name,
                    city.Country,
                    city.Population?.ToString() ?? "N/A",
                    city.Latitude.ToString(),
                    city.Longitude.ToString()
                );
            }

            AnsiConsole.Write(table);
            AnsiConsole.MarkupLine("[green]Нажмите любую клавишу для продолжения:[/]");
            _ = Console.ReadKey(intercept: true);
            Console.Clear();
            Console.WriteLine("\x1b[3J");
        }

        /// <summary>
        /// Выбор и вывод информации о городе
        /// </summary>
        /// <param name="cityCollection"></param>
        public void SelectAndDisplayCity(CityCollection cityCollection)
        {
            List<City> cities = cityCollection.Cities;

            if (cities.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]Список городов пуст.[/]");
                return;
            }
            List<string> cityNames = cities.Select(c => c.Name).ToList();
            cityNames.Add("Назад");
            string selectedCityName = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Выберите город:")
                    .AddChoices(cityNames));

            if (selectedCityName == "Назад")
            {
                return;
            }

            City? selectedCity = cities.FirstOrDefault(c => c.Name == selectedCityName);

            if (selectedCity != null)
            {
                DisplayCityInfo(selectedCity);
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Город не найден.[/]");
            }
        }
        /// <summary>
        /// Вывод полной информации о городе с API-запросами
        /// </summary>
        /// <param name="city">город</param>
        public void DisplayCityInfo(City city)
        {
            NominatimReverseResponse data = GeocodingFillers.ReverseGeocode(city.Latitude, city.Longitude);
            WeatherResponse weather = WeatherResponse.GetWeather(city.Latitude, city.Longitude);

            AnsiConsole.MarkupLine("[bold underline cyan]Информация о городе:[/]");
            AnsiConsole.MarkupLine($"[bold]Название:[/] {city.Name}");
            AnsiConsole.MarkupLine($"[bold]Страна:[/] {city.Country}");
            AnsiConsole.MarkupLine($"[bold]Население:[/] {(city.Population.HasValue ? city.Population.Value.ToString("N0") : "N/A")}");
            AnsiConsole.MarkupLine($"[bold]Координаты:[/] Широта: {city.Latitude}, Долгота: {city.Longitude}");

            // Вывод дополнительной информации из API геолокации
            if (data != null)
            {
                AnsiConsole.MarkupLine("[bold underline cyan]Дополнительная информация (обратное геокодирование по широте и долготе):[/]");
                AnsiConsole.MarkupLine($"[bold]Полное название:[/] {data.Display_Name}");
                AnsiConsole.MarkupLine($"[bold]Тип объекта:[/] {data.Name}");
                AnsiConsole.MarkupLine($"[bold]Адрес:[/] {data.Address?.Road} {data.Address?.House_Number}, {data.Address?.City}, {data.Address?.Country}");
                AnsiConsole.MarkupLine($"[bold]Почтовый индекс:[/] {data.Address?.Postcode}");
                AnsiConsole.MarkupLine($"[bold]Границы:[/] Широта: {data.Boundingbox?[0]} - {data.Boundingbox?[1]}, Долгота: {data.Boundingbox?[2]} - {data.Boundingbox?[3]}");
            }

            // Вывод информации о погоде
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
            _ = Console.ReadKey(intercept: true);
            Console.Clear();
            Console.WriteLine("\x1b[3J");
        }
        /// <summary>
        /// Вывод городов на карте
        /// </summary>
        /// <param name="cities"></param>
        public void DisplayCitiesOnMap(CityCollection cities)
        {
            try
            {
                Map.PrintMapWithPoints(cities.Cities);
            }
            catch (Exception)
            {
                throw;
            }
            AnsiConsole.MarkupLine("[green]Нажмите любую клавишу для продолжения:[/]");
            _ = Console.ReadKey(intercept: true);
            Console.Clear();
            Console.WriteLine("\x1b[3J");
        }
    }
}
