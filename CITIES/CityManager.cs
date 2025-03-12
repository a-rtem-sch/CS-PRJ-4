using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var name = AnsiConsole.Ask<string>("Введите название города:");
            var country = AnsiConsole.Ask<string>("Введите страну:");
            var population = AnsiConsole.Prompt(
                new TextPrompt<string>("Введите население (опционально, нажмите Enter чтобы пропустить):")
                    .AllowEmpty());
            var latitude = AnsiConsole.Ask<double>("Введите широту:");
            var longitude = AnsiConsole.Ask<double>("Введите долготу:");

            var city = new City
            {
                Name = name,
                Country = country,
                Population = string.IsNullOrEmpty(population) ? null : (long?)long.Parse(population),
                Latitude = latitude,
                Longitude = longitude
            };

            _cityCollection.AddCity(city);
            AnsiConsole.MarkupLine("[green]Город успешно добавлен. Нажмите любую класишу для продолжения:[/]");
            Console.ReadKey(intercept:true);
            Console.Clear();
        }

        public void EditCity()
        {
            var name = AnsiConsole.Ask<string>("Введите название города для редактирования:");
            var city = _cityCollection.GetCityByName(name);

            if (city == null)
            {
                AnsiConsole.MarkupLine("[red]Город не найден.[/]");
                return;
            }

            city.Name = AnsiConsole.Prompt(
                new TextPrompt<string>("Введите новое название города:")
                    .DefaultValue(city.Name));
            city.Country = AnsiConsole.Prompt(
                new TextPrompt<string>("Введите новую страну:")
                    .DefaultValue(city.Country));
            city.Population = long.Parse(AnsiConsole.Prompt(
                new TextPrompt<string>("Введите новое население (опционально, нажмите Enter чтобы пропустить):")
                    .AllowEmpty()
                    .DefaultValue(city.Population?.ToString() ?? "")));
            city.Latitude = AnsiConsole.Prompt(
                new TextPrompt<double>("Введите новую широту:")
                    .DefaultValue(city.Latitude));
            city.Longitude = AnsiConsole.Prompt(
                new TextPrompt<double>("Введите новую долготу:")
                    .DefaultValue(city.Longitude));

            AnsiConsole.MarkupLine("[green]Информация о городе успешно обновлена.[/]");
        }

        // Удаление города
        public void DeleteCity()
        {
            var name = AnsiConsole.Ask<string>("Введите название города для удаления:");
            _cityCollection.DeleteCity(name);
        }
    }
}
