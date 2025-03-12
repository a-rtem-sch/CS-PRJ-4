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
