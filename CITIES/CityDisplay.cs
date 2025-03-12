using Spectre.Console;

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
            AnsiConsole.MarkupLine("[bold underline]Информация о городе:[/]");
            AnsiConsole.MarkupLine($"[bold]Название:[/] {city.Name}");
            AnsiConsole.MarkupLine($"[bold]Страна:[/] {city.Country}");
            AnsiConsole.MarkupLine($"[bold]Население:[/] {(city.Population.HasValue ? city.Population.Value.ToString("N0") : "N/A")}");
            AnsiConsole.MarkupLine($"[bold]Координаты:[/] Широта: {city.Latitude}, Долгота: {city.Longitude}");
        }
    }
}
