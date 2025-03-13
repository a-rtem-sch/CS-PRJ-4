using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
