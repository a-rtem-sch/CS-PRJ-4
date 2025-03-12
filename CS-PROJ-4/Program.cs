using Spectre.Console;
using CITIES;
using PARSING;
using System.Text;

namespace CS_PROJ_4
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;


            AnsiConsole.MarkupLine("[bold green]Интерактивный справочник городов[/]");

            

            var fileHandler = new CityFileHandler();
            var filePath = AnsiConsole.Ask<string>("Введите путь к файлу с данными о городах:").Trim('\"');

            if (!File.Exists(filePath))
            {
                AnsiConsole.MarkupLine("[red]Файл не найден.[/]");
                return;
            }
            Console.Clear();
            var readerOutput = fileHandler.ReadCitiesFromFile(filePath);
            var cities = readerOutput.Item1;
            var errorLines = readerOutput.Item2;
            var cityCollection = new CityCollection(cities);
            var cityManager = new CityManager(cityCollection);
            var cityDisplay = new CityDisplay(cityCollection);

            while (true)
            {
                var choices = new List<string>
                {
                    "Просмотреть список городов",
                    "Выбрать город",
                    "Добавить город",
                    "Редактировать город",
                    "Удалить город"
                };

                if (errorLines.Count > 0)
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
                    case "Выбрать город":
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
                        foreach (string s in errorLines)
                        {
                            AnsiConsole.MarkupLine($"Строка:: [yellow]\"{s.Split("#")[0]}\"[/], причина:: {s.Split("#")[1]}");
                        }
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
