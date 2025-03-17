using CITIES;
using PARSING;
using Spectre.Console;
using System.Text;


namespace CS_PROJ_4
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            List<City>? cities = null;
            List<BadRecord>? badRecords = null;
            string? filePath = string.Empty;

            var cityCollection = new CityCollection();

            GeneralParsing.GetFile(ref cityCollection, ref badRecords, ref filePath);

            
            var cityManager = new CityManager(cityCollection);
            var cityDisplay = new CityDisplay(cityCollection);
            //var map = new Map();

            while (true)
            {
                var choices = new List<string>
                {
                    "Просмотреть список городов",
                    "Информация о городе",
                    "Города на карте",
                    "Добавить город",
                    "Редактировать город",
                    "Удалить город"
                };

                if (badRecords != null && badRecords.Count > 0)
                {
                    choices.Add("Просмотреть пропущенные строки");
                }
                choices.Add("Ввести новые данные");
                choices.Add("Сохранить");
                choices.Add("Выйти");
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Выберите действие:")
                        .AddChoices(choices));

                switch (choice)
                {
                    case "Просмотреть список городов":
                        try { cityDisplay.DisplayCitiesTable(); }
                        catch { AnsiConsole.MarkupLine("[red]Произошла неизвестная ошибка:: ошибка показа списка[/]");
                            AnsiConsole.MarkupLine("[red]Нажмите любую клавишу для продолжения:[/]");
                            Console.ReadKey(intercept: true);
                        }
                        break;
                    case "Информация о городе":
                        try { cityDisplay.SelectAndDisplayCity(cityCollection); }
                        catch { AnsiConsole.MarkupLine("[red]Произошла неизвестная ошибка:: ошибка показа информации[/]");
                            AnsiConsole.MarkupLine("[red]Нажмите любую клавишу для продолжения:[/]");
                            Console.ReadKey(intercept: true);
                        }
                        break;
                    case "Города на карте":
                        try { cityDisplay.DisplayCitiesOnMap(cityCollection); }
                        catch { AnsiConsole.MarkupLine("[red]Произошла неизвестная ошибка:: ошибка отображения на карте[/]");
                            AnsiConsole.MarkupLine("[red]Нажмите любую клавишу для продолжения:[/]");
                            Console.ReadKey(intercept: true);
                        }
                        break;
                    case "Добавить город":
                        try { cityManager.AddCity(); }
                        catch { AnsiConsole.MarkupLine("[red]Произошла неизвестная ошибка:: ошибка добавления[/]");
                            AnsiConsole.MarkupLine("[red]Нажмите любую клавишу для продолжения:[/]");
                            Console.ReadKey(intercept: true);
                        }
                        break;
                    case "Редактировать город":
                        try { cityManager.EditCity(cityCollection); }
                        catch { AnsiConsole.MarkupLine("[red]Произошла неизвестная ошибка:: ошибка редактирования[/]");
                            AnsiConsole.MarkupLine("[red]Нажмите любую клавишу для продолжения:[/]");
                            Console.ReadKey(intercept: true);
                        }
                        break;
                    case "Удалить город":
                        try { cityManager.DeleteCity(); }
                        catch { AnsiConsole.MarkupLine("[red]Произошла неизвестная ошибка:: ошибка удаления[/]");
                            AnsiConsole.MarkupLine("[red]Нажмите любую клавишу для продолжения:[/]");
                            Console.ReadKey(intercept: true);
                        }
                        break;
                    case "Просмотреть пропущенные строки":
                        Console.Clear();
                        Console.WriteLine("\x1b[3J");
                        foreach (BadRecord br in badRecords)
                        {
                            try
                            {
                                AnsiConsole.MarkupLine($"Строка:: [bold]\"{br.RawRecord}\"[/]");
                            }
                            catch { AnsiConsole.MarkupLine("[red]Произошла неизвестная ошибка:: ошибка просмотра пропущенных строк[/]");
                                AnsiConsole.MarkupLine("[red]Нажмите любую клавишу для продолжения:[/]");
                                Console.ReadKey(intercept: true);
                            }

                        }
                        AnsiConsole.MarkupLine("[green]Нажмите любую клавишу для продолжения:[/]");
                        Console.ReadKey(intercept: true);
                        Console.Clear();
                        Console.WriteLine("\x1b[3J");
                        
                        break;
                    case "Ввести новые данные":
                        try { GeneralParsing.GetFile(ref cityCollection, ref badRecords, ref filePath); }
                        catch { AnsiConsole.MarkupLine("[red]Произошла неизвестная ошибка:: ошибка ввода данных[/]");
                            AnsiConsole.MarkupLine("[red]Нажмите любую клавишу для продолжения:[/]");
                            Console.ReadKey(intercept: true);
                        }
                        break;
                    case "Сохранить":
                        try
                        {
                            string fileToWriteFormat = GeneralParsing.ChooseMarkdown();
                            if (fileToWriteFormat == "JSON")
                            {
                                JSONHandler.ExportCitiesToJson(cities, filePath);
                            }
                            else
                            {
                                CSVHandler.ExportCitiesToCsv(cities, filePath);
                            }
                        }
                        catch { AnsiConsole.MarkupLine("[red]Произошла неизвестная ошибка:: ошибка сохранения[/]");
                            AnsiConsole.MarkupLine("[red]Нажмите любую клавишу для продолжения:[/]");
                            Console.ReadKey(intercept: true);
                        }
                        break;
                    case "Выйти":
                        bool isConfirmed = AnsiConsole.Confirm("Вы уверены, что хотите выйти без автосохранения?");

                        if (isConfirmed)
                        {

                            AnsiConsole.MarkupLine("[green]Выход...[/]");
                            return;
                        }
                        else
                        {
                            break;
                        }

                }
            }
        }
    }
}
