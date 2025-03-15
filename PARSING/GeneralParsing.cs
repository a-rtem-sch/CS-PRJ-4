using CITIES;
using Spectre.Console;
using System.Text.RegularExpressions;

namespace PARSING
{
    public class GeneralParsing
    {
        public static string ChooseMarkdown()
        {
            string? fileFormat = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Выберите тип разметки файла:")
                        .AddChoices("CSV", "JSON"));
            return fileFormat;
        }
        public static string ExtractRawRecord(string exceptionMessage)
        {
            // Регулярное выражение для поиска RawRecord
            var regex = new Regex(@"RawRecord:\s*(.+?)(\r?\n|$)", RegexOptions.Singleline);
            var match = regex.Match(exceptionMessage);

            if (match.Success)
            {
                // Возвращаем найденный RawRecord, убирая лишние пробелы
                return match.Groups[1].Value.Trim();
            }

            return "N/A"; // Если RawRecord не найден
        }

        public static void GetFile(ref List<City> cities, ref List<BadRecord> badRecords, ref string? filePath)
        {

            while (true)
            {
                filePath = AnsiConsole.Ask<string>("Введите путь к файлу с данными о городах:").Trim('\"');

                if (!File.Exists(filePath))
                {
                    AnsiConsole.MarkupLine("[red]Файл не найден. Пожалуйста, введите путь ещё раз.[/]");
                    continue;
                }

                Console.Clear();

                string fileFormat = ChooseMarkdown();

                if (fileFormat == "CSV")
                {
                    try
                    {
                        var result = CSVHandler.ImportCitiesFromCsv(filePath);
                        cities = result.Cities;
                        badRecords = result.BadRecords;
                        break;
                    }
                    catch
                    {
                        AnsiConsole.MarkupLine("[red]Критическая несовместимость файловой разметки с заявленной! Пожалуйста, выберите другой файл.[/]");
                    }
                }
                else
                {
                    try
                    {
                        cities = JSONHandler.ImportCitiesFromJson(filePath);
                        break;
                    }
                    catch
                    {
                        AnsiConsole.MarkupLine("[red]Критическая несовместимость файловой разметки с заявленной! Пожалуйста, выберите другой файл.[/]");
                    }
                }
            }

            if (badRecords != null && badRecords.Count > 0)
            {
                AnsiConsole.MarkupLine("[yellow]Некоторые строки не были обработаны! \nПодробнее в разделе \"Просмотреть пропущенные строки\"[/]");
            }
        }
    }
}
