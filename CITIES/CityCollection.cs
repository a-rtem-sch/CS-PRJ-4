using Spectre.Console;

namespace CITIES
{
    /// <summary>
    /// Коллекция городов
    /// </summary>
    public class CityCollection(List<City> cities = null)
    {
        // основной список городов
        public List<City> Cities { get; private set; } = cities ?? [];

        public void AddCity(City city)
        {
            Cities.Add(city);
        }

        public void UpdateCities(List<City> newCities)
        {
            Cities = newCities;
        }


        public void DeleteCity(string name)
        {
            City? city = Cities.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (city != null)
            {
                _ = Cities.Remove(city);
                //AnsiConsole.MarkupLine("[green]Город успешно удален.[/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Город не найден.[/]");
                
            }
        }

        public City GetCityByName(string name)
        {
            return Cities.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

    }
}
