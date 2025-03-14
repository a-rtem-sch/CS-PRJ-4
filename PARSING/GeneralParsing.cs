using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
    }
}
