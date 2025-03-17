using CITIES;
using Spectre.Console;
using System.Text.Json;
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

        public static void GetFile(ref CityCollection cityCollection, ref List<BadRecord> badRecords, ref string? filePath)
        {

            while (true)
            {
                char[] charsToTrim = { '\"', ' ' };
                filePath = AnsiConsole.Ask<string>("Введите путь к файлу с данными о городах:").Trim(charsToTrim);

                if (!File.Exists(filePath))
                {
                    AnsiConsole.MarkupLine("[red]Файл не найден. Пожалуйста, введите путь ещё раз.[/]");
                    continue;
                }

                Console.Clear();
                Console.WriteLine("\x1b[3J");

                string fileFormat = ChooseMarkdown();

                if (fileFormat == "CSV")
                {
                    try
                    {
                        var result = CSVHandler.ImportCitiesFromCsv(filePath);
                        cityCollection.UpdateCities(result.Cities);
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
                        var result = JSONHandler.ImportCitiesFromJson(filePath);
                        cityCollection.UpdateCities(result.Cities);
                        badRecords = result.BadRecords;
                        break;
                    }
                    catch (JsonException)
                    {
                        AnsiConsole.MarkupLine($"[red]Ошибка сериализации файла![/]");
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

    public class BadRecord
    {
        public BadRecord()
        {
            Field = string.Empty;
            RawRecord = string.Empty;
            ErrorMessage = string.Empty;
        }
        public string RawRecord { get; set; }
        public string Field { get; set; }
        public string ErrorMessage { get; set; }
    }
}
