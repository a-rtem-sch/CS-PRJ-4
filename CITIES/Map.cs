using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using Spectre.Console;

namespace CITIES
{
    /// <summary>
    /// Класс для работы с картой
    /// </summary>
    public class Map
    {
        // размеры карты
        private const int MapCols = 69;
        private const int MapRows = 41;


        // Границы проекции Меркатора
        private static readonly double[] XRange = { -20037508.34, 20037508.34 };
        private static readonly double[] YRange = { -20048966.10, 20048966.10 };



        // Преобразование координат в координаты карты
        private static (int x, int y) LatLonToXY(double lat, double lon)
        {
            // Инициализация систем координат
            var crsFrom = GeographicCoordinateSystem.WGS84; // WGS84 (широта/долгота)
            var crsTo = ProjectedCoordinateSystem.WebMercator; // Web Mercator

            var transform = new CoordinateTransformationFactory().CreateFromCoordinateSystems(crsFrom, crsTo);

            // Преобразование координат
            var (x, y) = transform.MathTransform.Transform(lon, lat);

            // Нормализация координат в диапазоны
            double xPercent = (x - XRange[0]) / (XRange[1] - XRange[0]);
            double yPercent = (y - YRange[0]) / (YRange[1] - YRange[0]);

            // Преобразование в координаты карты
            int mapX = (int)(xPercent * MapCols);
            int mapY = (int)(yPercent * MapRows);

            // Инвертирование Y (так как координаты карты идут сверху вниз)
            mapY = InverseNumInRange(mapY, 0, MapRows);

            // Проверка на выход за границы карты
            int topMargin = 10;
            int bottomMargin = 10;
            if (mapY - topMargin < 0 || mapY > MapRows - bottomMargin)
            {
                throw new ArgumentException($"Координаты ({lat}, {lon}) выходят за пределы карты.");
            }

            return (mapX, mapY - topMargin);
        }

        // Инвертирование числа в диапазоне
        private static int InverseNumInRange(int num, int minNum, int maxNum)
        {
            return (maxNum + minNum) - num;
        }

        // Отображение карты с точками
        public static void PrintMapWithPoints(List<City> cities)
        {
            string[] mapLines = WorldMap.Split('\n');
            int counter = 1;
            foreach (var city in cities)
            {
                city.Marker = counter++.ToString();
                try
                {
                    var (x, y) = LatLonToXY(city.Latitude, city.Longitude);
                    x += 8; // Смещение на 8 символов вправо

                    if (y >= 0 && y < mapLines.Length && x >= 0 && x + city.Marker.Length <= mapLines[y].Length)
                    {
                        // Вставляем маркер на карту
                        char[] line = mapLines[y].ToCharArray();
                        for (int i = 0; i < city.Marker.Length; i++)
                        {
                            line[x + i] = city.Marker[i];
                        }
                        mapLines[y] = new string(line);
                        AnsiConsole.MarkupLine($"{city.Marker}) [bold]{city.Name}[/]");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Ошибка для города {city.Name}: {e.Message}");
                }
            }


            // Вывод карты с цветными маркерами
            foreach (var line in mapLines)
            {
                for (int i = 0; i < line.Length; i++)
                {
                    bool isMarker = false;
                    foreach (var city in cities)
                    {
                        if (i + city.Marker.Length <= line.Length && line.Substring(i, city.Marker.Length) == city.Marker)
                        {
                            Console.ForegroundColor = ConsoleColor.Red; // Устанавливаем цвет маркера
                            Console.Write(city.Marker);
                            Console.ResetColor(); // Возвращаем стандартный цвет
                            i += city.Marker.Length - 1; // Пропускаем оставшиеся символы маркера
                            isMarker = true;
                            break;
                        }
                    }

                    if (!isMarker)
                    {
                        Console.Write(line[i]); // Обычный символ карты
                    }
                }
                Console.WriteLine(); // Переход на новую строку
            }
        }

        /// <summary>
        /// Карта для отрисовка, константа
        /// </summary>
        public const string WorldMap = @"          . _..::__:  ,-""-""._       |]       ,     _,.__              
          _.___ _ _<_>`!(._`.`-.    /        _._     `_ ,_/  '  '-._.---.-.__ 
        .{     "" "" `-==,',._\{  \  / {)     / _ "">_,-' `                 /-/_ 
         \_.:--.       `._ )`^-. ""'      , [_/(                       __,/-'  
        '""'     \         ""    _L       |-_,--'                )     /. (|    
                 |           ,'         _)_.\\._<> {}              _,' /  '   
                 `.         /          [_/_'` `""(                <'}  )       
                  \\    .-. )          /   `-'""..' `:._          _)  '        
           `        \  (  `(          /         `:\  > /  ,-^.  /' '          
                     `._,   """"        |           \`'   \|   ?_)  {\          
                        `=.---.       `._._       ,'     ""`  |' ,- '.         
                          |    `-._        |     /          `:`<_|=--._       
                          (        >       .     | ,          `=.__.`-'\      
                           `.     /        |     |{|              ,-.,\     . 
                            |   ,'          \   / `'            ,""     \      
                            |  /             |_'                |  __  /      
                            | |                                 '-'  `-'   \. 
                            |/                                        ""    /  
                            \.                                            '   

                             ,/           ______._.--._ _..---.---------.     
        __,-----""-..?----_/ )\    . ,-'""             ""                  (__--/
                              /__/\/                                          ";



        //    const string WorldMap =
        //    @"⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣀⣄⣠⣀⡀⣀⣠⣤⣤⣤⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
        //    ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣄⢠⣠⣼⣿⣿⣿⣟⣿⣿⣿⣿⣿⣿⣿⣿⡿⠋⠀⠀⠀⢠⣤⣦⡄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠰⢦⣄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
        //    ⠀⠀⠀⠀⠀⠀⠀⠀⣼⣿⣟⣾⣿⣽⣿⣿⣅⠈⠉⠻⣿⣿⣿⣿⣿⡿⠇⠀⠀⠀⠀⠀⠉⠀⠀⠀⠀⠀⢀⡶⠒⢉⡀⢠⣤⣶⣶⣿⣷⣆⣀⡀⠀⢲⣖⠒⠀⠀⠀⠀⠀⠀⠀
        //    ⢀⣤⣾⣶⣦⣤⣤⣶⣿⣿⣿⣿⣿⣿⣽⡿⠻⣷⣀⠀⢻⣿⣿⣿⡿⠟⠀⠀⠀⠀⠀⠀⣤⣶⣶⣤⣀⣀⣬⣷⣦⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣶⣦⣤⣦⣼⣀⠀
        //    ⠈⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡿⠛⠓⣿⣿⠟⠁⠘⣿⡟⠁⠀⠘⠛⠁⠀⠀⢠⣾⣿⢿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡿⠏⠙⠁
        //    ⠀⠸⠟⠋⠀⠈⠙⣿⣿⣿⣿⣿⣿⣷⣦⡄⣿⣿⣿⣆⠀⠀⠀⠀⠀⠀⠀⠀⣼⣆⢘⣿⣯⣼⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡉⠉⢱⡿⠀⠀⠀⠀⠀
        //    ⠀⠀⠀⠀⠀⠀⠀⠘⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣟⡿⠦⠀⠀⠀⠀⠀⠀⠀⠙⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡿⡗⠀⠈⠀⠀⠀⠀⠀⠀
        //    ⠀⠀⠀⠀⠀⠀⠀⠀⢻⣿⣿⣿⣿⣿⣿⣿⣿⠋⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⢿⣿⣉⣿⡿⢿⢷⣾⣾⣿⣞⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⠋⣠⠟⠀⠀⠀⠀⠀⠀⠀⠀
        //    ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠹⣿⣿⣿⠿⠿⣿⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⣾⣿⣿⣷⣦⣶⣦⣼⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣷⠈⠛⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀
        //    ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠻⣿⣤⡖⠛⠶⠤⡀⠀⠀⠀⠀⠀⠀⠀⢰⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡿⠁⠙⣿⣿⠿⢻⣿⣿⡿⠋⢩⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
        //    ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠙⠧⣤⣦⣤⣄⡀⠀⠀⠀⠀⠀⠘⢿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡇⠀⠀⠀⠘⣧⠀⠈⣹⡻⠇⢀⣿⡆⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
        //    ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⣿⣿⣿⣿⣿⣤⣀⡀⠀⠀⠀⠀⠀⠀⠈⢽⣿⣿⣿⣿⣿⠋⠀⠀⠀⠀⠀⠀⠀⠀⠹⣷⣴⣿⣷⢲⣦⣤⡀⢀⡀⠀⠀⠀⠀⠀⠀
        //    ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⢿⣿⣿⣿⣿⣿⣿⠟⠀⠀⠀⠀⠀⠀⠀⢸⣿⣿⣿⣿⣷⢀⡄⠀⠀⠀⠀⠀⠀⠀⠀⠈⠉⠂⠛⣆⣤⡜⣟⠋⠙⠂⠀⠀⠀⠀⠀
        //    ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢹⣿⣿⣿⣿⠟⠀⠀⠀⠀⠀⠀⠀⠀⠘⣿⣿⣿⣿⠉⣿⠃⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣤⣾⣿⣿⣿⣿⣿⣆⠀⠰⠄⠀⠉⠀⠀
        //    ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣸⣿⣿⡿⠃⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢹⣿⡿⠃⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢻⣿⠿⠿⣿⣿⣿⠇⠀⠀⢀⠀⠀⠀
        //    ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣿⡿⠛⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⢻⡇⠀⠀⢀⣼⠗⠀⠀
        //    ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⣿⠃⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⠁⠀⠀⠀
        //    ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⠒⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀";

        //}
    }
}