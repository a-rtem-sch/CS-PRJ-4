using Spectre.Console;

namespace CITIES
{
    public class CityCollection
    {
        public List<City> Cities { get; private set; }

        // Конструктор для инициализации коллекции
        public CityCollection(List<City> cities = null)
        {
            Cities = cities ?? new List<City>();
        }

        // Добавление города
        public void AddCity(City city)
        {
            Cities.Add(city);
        }

        // Редактирование города
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

        // Удаление города
        public void DeleteCity(string name)
        {
            var city = Cities.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (city != null)
            {
                Cities.Remove(city);
                AnsiConsole.MarkupLine("[green]Город успешно удален.[/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Город не найден.[/]");
            }
        }

        // Поиск города по названию
        public City GetCityByName(string name)
        {
            return Cities.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
