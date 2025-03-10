using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITIES
{
    public class CityDisplay
    {
        private readonly CityCollection _cityCollection;

        public CityDisplay(CityCollection cityCollection)
        {
            _cityCollection = cityCollection;
        }

        // Отображение списка городов
        public void DisplayCities()
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

            AnsiConsole.Render(table);
        }
    }
}
