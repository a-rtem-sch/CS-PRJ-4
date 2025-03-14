using Spectre.Console;
using CITIES;
using PARSING;
using System.Text;
using GEOCODING;

namespace CS_PROJ_4
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            
            string? filePath = AnsiConsole.Ask<string>("Введите путь к файлу с данными о городах:").Trim('\"');

            if (!File.Exists(filePath))
            {
                AnsiConsole.MarkupLine("[red]Файл не найден.[/]");
                return;
            }
            Console.Clear();

            string fileFormat = GeneralParsing.ChooseMarkdown();

            List<City>? cities = new();
            List<BadRecord>? badRecords = new();

            if (fileFormat == "CSV")
            {
                var result = CSVHandler.ImportCitiesFromCsv(filePath);
                cities = result.Cities;
                badRecords = result.BadRecords;
            }
            else
            {
                cities = JSONHandler.ImportCitiesFromJson(filePath);
            }
            
            var cityCollection = new CityCollection(cities);
            var cityManager = new CityManager(cityCollection);
            var cityDisplay = new CityDisplay(cityCollection);

            while (true)
            {
                var choices = new List<string>
                {
                    "Просмотреть список городов",
                    "Информация о городе",
                    "Добавить город",
                    "Редактировать город",
                    "Удалить город"
                };

                if (badRecords.Count > 0)
                {
                    choices.Add("Просмотреть пропущенные строки");
                }
                choices.Add("Выйти и сохранить");
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Выберите действие:")
                        .AddChoices(choices));

                switch (choice)
                {
                    case "Просмотреть список городов":
                        cityDisplay.DisplayCitiesTable();
                        break;
                    case "Информация о городе":
                        cityDisplay.SelectAndDisplayCity(cityCollection);
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
                    case "Просмотреть пропущенные строки":
                        Console.Clear();
                        foreach (BadRecord br in badRecords)
                        {
                            AnsiConsole.MarkupLine($"Строка:: [yellow]\"{br.RawRecord}, {br.Field}");
                        }
                        AnsiConsole.MarkupLine("[green]Нажмите любую класишу для продолжения:[/]");
                        Console.ReadKey(intercept: true);
                        Console.Clear();
                        break;
                    case "Выйти и сохранить":
                        //fileHandler.SaveCitiesToFile(filePath, cityCollection.Cities);
                        string fileToWriteFormat = GeneralParsing.ChooseMarkdown();
                        if (fileToWriteFormat == "JSON")
                        {
                            JSONHandler.ExportCitiesToJson(cities, filePath);
                        }
                        else
                        {
                            CSVHandler.ExportCitiesToCsv(cities, filePath);
                        }
                        AnsiConsole.MarkupLine("[green]Выход...[/]");
                        return;
                }
            }
        }
    }
}
