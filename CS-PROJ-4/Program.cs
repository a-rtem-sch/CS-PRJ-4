using Spectre.Console;
using CITIES;
using PARSING;

namespace CS_PROJ_4
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            AnsiConsole.MarkupLine("[bold green]Интерактивный справочник городов[/]");

            var fileHandler = new CityFileHandler();
            var filePath = AnsiConsole.Ask<string>("Введите путь к файлу с данными о городах:");

            if (!File.Exists(filePath.Trim('\"')))
            {
                AnsiConsole.MarkupLine("[red]Файл не найден.[/]");
                return;
            }

            var cities = fileHandler.ReadCitiesFromFile(filePath);
            var cityCollection = new CityCollection(cities);
            var cityManager = new CityManager(cityCollection);
            var cityDisplay = new CityDisplay(cityCollection);

            while (true)
            {
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Выберите действие:")
                        .AddChoices(new[] { "Просмотреть список городов", "Добавить город", "Редактировать город", "Удалить город", "Выйти и сохранить" }));

                switch (choice)
                {
                    case "Просмотреть список городов":
                        cityDisplay.DisplayCities();
                        break;
                    case "Добавить город":
                        cityManager.AddCity();
                        break;
                    case "Редактировать город":
                        cityManager.EditCity();
                        break;
                    case "Удалить город":
                        cityManager.DeleteCity();
                        break;
                    case "Выйти и сохранить":
                        fileHandler.SaveCitiesToFile(filePath, cityCollection.Cities);
                        AnsiConsole.MarkupLine("[green]Выход...[/]");
                        return;
                }
            }
        }
    }
}
