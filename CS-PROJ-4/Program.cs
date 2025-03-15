using CITIES;
using PARSING;
using Spectre.Console;

namespace CS_PROJ_4
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            //Console.OutputEncoding = Encoding.UTF8;

            //Map map = new Map();

            ////map.PrintMap();

            //// Пример добавления города
            //double latitude = 55.7558; // Широта Москвы
            //double longitude = 37.6176; // Долгота Москвы

            ////string modifiedMap = map.AddCityToMap(latitude, longitude);
            ////Console.WriteLine(modifiedMap);

            //Console.ReadKey();
            //string mapString = Map.GetMapString();

            //// Тестовые координаты
            //var coords = new (double lat, double lon, string label, char marker)[]
            //{
            //(51.369208,  0.106923, "London", '1'),
            //(39.768173, -74.715499, "New York", '2'),
            //(-18.259349, 46.850023, "Madagascar", '3'),
            //(-34.520136, 137.565312, "Melbourne", '4'),
            //(48.208176, 16.373819, "Vienna", '5'),
            //(-43.532055, 172.636230, "CHCH", '6'),
            //(35.689487, 139.691711, "Tokyo", '7'),
            //(33.929047, -118.441495, "LA", '8'),
            //(49.240186, -123.110419, "Vancouver", '9')
            //};

            // Отображение карты с точками
            //Map.PrintMapWithPoints(mapString, coords);





            List<City>? cities = null;
            List<BadRecord>? badRecords = null;
            string? filePath = string.Empty;

            GeneralParsing.GetFile(ref cities, ref badRecords, ref filePath);

            var cityCollection = new CityCollection(cities);
            var cityManager = new CityManager(cityCollection);
            var cityDisplay = new CityDisplay(cityCollection);
            //var map = new Map();

            while (true)
            {
                var choices = new List<string>
                {
                    "Ввести новые данные",
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
                choices.Add("Выйти и сохранить");
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Выберите действие:")
                        .AddChoices(choices));

                switch (choice)
                {
                    case "Ввести новые данные":
                        GeneralParsing.GetFile(ref cities, ref badRecords, ref filePath);
                        break;
                    case "Просмотреть список городов":
                        cityDisplay.DisplayCitiesTable();
                        break;
                    case "Информация о городе":
                        cityDisplay.SelectAndDisplayCity(cityCollection);
                        break;
                    case "Города на карте":
                        cityDisplay.DisplayCitiesOnMap(cityCollection);
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
                            AnsiConsole.WriteLine($"Строка:: \"{br.RawRecord}\"");
                        }
                        AnsiConsole.MarkupLine("[green]Нажмите любую клавишу для продолжения:[/]");
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
