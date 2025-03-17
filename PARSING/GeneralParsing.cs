using CITIES;
using Spectre.Console;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace PARSING
{
    /// <summary>
    /// Глобальный класс, предоставляющий методы для парсинга
    /// </summary>
    public class GeneralParsing
    {
        /// <summary>
        /// Выбор разметки
        /// </summary>
        /// <returns></returns>
        public static string ChooseMarkdown()
        {
            string? fileFormat = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Выберите тип разметки файла:")
                        .AddChoices("CSV", "JSON"));
            return fileFormat;
        }
        /// <summary>
        /// Служебный класс для обработки Bad Record для вывода ошибки
        /// </summary>
        /// <param name="exceptionMessage"></param>
        /// <returns></returns>
        public static string ExtractRawRecord(string exceptionMessage)
        {
            // Регулярное выражение для поиска RawRecord
            Regex regex = new (@"RawRecord:\s*(.+?)(\r?\n|$)", RegexOptions.Singleline);
            Match match = regex.Match(exceptionMessage);

            if (match.Success)
            {
                // Возвращаем найденный RawRecord, убирая лишние пробелы
                return match.Groups[1].Value.Trim();
            }

            return "N/A"; // Если RawRecord не найден
        }
        /// <summary>
        /// Получить файл у пользователя
        /// </summary>
        /// <param name="cityCollection">Города</param>
        /// <param name="badRecords">Ошибки</param>
        /// <param name="filePath">Путь к файлу</param>
        public static void GetFile(ref CityCollection cityCollection, ref List<BadRecord> badRecords, ref string? filePath)
        {

            while (true)
            {
                char[] charsToTrim = ['\"', ' '];
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
                        (List<City> cities, List<BadRecord> badRecords1) = CSVHandler.ImportCitiesFromCsv(filePath);
                        cityCollection.UpdateCities(cities);
                        badRecords = badRecords1;
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
                        (List<City> cities, List<BadRecord> badRecords1) = JSONHandler.ImportCitiesFromJson(filePath);
                        cityCollection.UpdateCities(cities);
                        badRecords = badRecords1;
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

    /// <summary>
    /// Запись об ошибке при парсинге для логгирования
    /// </summary>
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
