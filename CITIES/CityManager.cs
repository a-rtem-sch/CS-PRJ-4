﻿using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GEOCODING;
using System.Net.Http.Json;
using System.Globalization;

namespace CITIES
{
    public class CityManager
    {
        private readonly CityCollection _cityCollection;

        public CityManager(CityCollection cityCollection)
        {
            _cityCollection = cityCollection;
        }

        public void AddCity()
        {
            AddCityAsync().GetAwaiter().GetResult();
        }
        public async Task AddCityAsync()
        {
            var name = AnsiConsole.Ask<string>("Введите название города:");
            var country = AnsiConsole.Ask<string>("Введите страну:");
            
            ulong? population = GetPopulationInput();


            // Автоматическое определение координат
            var coordinates = await GeocodingService.GeocodeAsync($"{name}, {country}");

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

            City city = new City
            {
                Name = name,
                Country = country,
                Population = population,
                Latitude = latitude,
                Longitude = longitude
            };

            _cityCollection.AddCity(city);
            AnsiConsole.MarkupLine("[green]Город успешно добавлен. Нажмите любую клавишу для продолжения:[/]");
            Console.ReadKey(intercept: true);
            Console.Clear();
        }


        ulong? GetPopulationInput()
        {
            while (true)
            {
                string populationInput = AnsiConsole.Prompt(
                    new TextPrompt<string>("Введите население (опционально, нажмите Enter чтобы пропустить):")
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

        

        public void EditCity()
        {
            //string name = AnsiConsole.Ask<string>("Введите название города для редактирования:");
            //City city = _cityCollection.GetCityByName(name);

            var cityNames = _cityCollection.Cities.Select(c => c.Name).ToList();
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
            city.Population = GetPopulationInput();
            city.Latitude = AnsiConsole.Prompt(
                new TextPrompt<double>("Введите новую широту:")
                    .DefaultValue(city.Latitude));
            city.Longitude = AnsiConsole.Prompt(
                new TextPrompt<double>("Введите новую долготу:")
                    .DefaultValue(city.Longitude));

            AnsiConsole.MarkupLine("[green]Информация о городе успешно обновлена. Нажмите любую класишу для продолжения:[/]");
            Console.ReadKey(intercept: true);
            Console.Clear();
        }

        // Удаление города
        public void DeleteCity()
        {
            var cityNames = _cityCollection.Cities.Select(c => c.Name).ToList();
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
            AnsiConsole.MarkupLine("[green]Город успешно удален. Нажмите любую класишу для продолжения:[/]");
            Console.ReadKey(intercept: true);
            Console.Clear();
        }
    }
}
