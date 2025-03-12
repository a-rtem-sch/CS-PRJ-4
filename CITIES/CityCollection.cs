using Spectre.Console;

namespace CITIES
{
    public class CityCollection
    {
        public List<City> Cities { get; private set; }

        public CityCollection(List<City> cities = null)
        {
            Cities = cities ?? new List<City>();
        }

        public void AddCity(City city)
        {
            Cities.Add(city);
        }
        public void EditCity(string name, Action<City> editAction)
        {
            var city = Cities.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (city != null)
            {
                editAction(city);
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Город не найден.[/]");
            }
        }

        public void DeleteCity(string name)
        {
            var city = Cities.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (city != null)
            {
                Cities.Remove(city);
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
