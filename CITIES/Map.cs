using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;

namespace CITIES
{
    public class Map
    {

        private const int MapCols = 69;
        private const int MapRows = 41;

        // Границы проекции Меркатора
        private static readonly double[] XRange = { -20037508.34, 20037508.34 };
        private static readonly double[] YRange = { -20048966.10, 20048966.10 };

        public static string GetMapString()
        {
            return WorldMap;
        }

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
        public static void PrintMapWithPoints(string mapString, (double lat, double lon, string label, char marker)[] points)
        {
            string[] mapLines = mapString.Split('\n');

            foreach (var point in points)
            {
                try
                {
                    var (x, y) = LatLonToXY(point.lat, point.lon);
                    x += 8;
                    if (y >= 0 && y < mapLines.Length && x >= 0 && x < mapLines[y].Length)
                    {
                        char[] line = mapLines[y].ToCharArray();
                        line[x] = point.marker;
                        mapLines[y] = new string(line);
                        Console.WriteLine($"{point.marker}) {point.label} ({point.lat}, {point.lon}) -> ({x}, {y})");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Ошибка: {e.Message}");
                }
            }

            // Вывод карты
            foreach (var line in mapLines)
            {
                Console.WriteLine(line);
            }
        }




        //        const string World =
        //        @"⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣀⣄⣠⣀⡀⣀⣠⣤⣤⣤⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
        //⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣄⢠⣠⣼⣿⣿⣿⣟⣿⣿⣿⣿⣿⣿⣿⣿⡿⠋⠀⠀⠀⢠⣤⣦⡄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠰⢦⣄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
        //⠀⠀⠀⠀⠀⠀⠀⠀⣼⣿⣟⣾⣿⣽⣿⣿⣅⠈⠉⠻⣿⣿⣿⣿⣿⡿⠇⠀⠀⠀⠀⠀⠉⠀⠀⠀⠀⠀⢀⡶⠒⢉⡀⢠⣤⣶⣶⣿⣷⣆⣀⡀⠀⢲⣖⠒⠀⠀⠀⠀⠀⠀⠀
        //⢀⣤⣾⣶⣦⣤⣤⣶⣿⣿⣿⣿⣿⣿⣽⡿⠻⣷⣀⠀⢻⣿⣿⣿⡿⠟⠀⠀⠀⠀⠀⠀⣤⣶⣶⣤⣀⣀⣬⣷⣦⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣶⣦⣤⣦⣼⣀⠀
        //⠈⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡿⠛⠓⣿⣿⠟⠁⠘⣿⡟⠁⠀⠘⠛⠁⠀⠀⢠⣾⣿⢿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡿⠏⠙⠁
        //⠀⠸⠟⠋⠀⠈⠙⣿⣿⣿⣿⣿⣿⣷⣦⡄⣿⣿⣿⣆⠀⠀⠀⠀⠀⠀⠀⠀⣼⣆⢘⣿⣯⣼⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡉⠉⢱⡿⠀⠀⠀⠀⠀
        //⠀⠀⠀⠀⠀⠀⠀⠘⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣟⡿⠦⠀⠀⠀⠀⠀⠀⠀⠙⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡿⡗⠀⠈⠀⠀⠀⠀⠀⠀
        //⠀⠀⠀⠀⠀⠀⠀⠀⢻⣿⣿⣿⣿⣿⣿⣿⣿⠋⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⢿⣿⣉⣿⡿⢿⢷⣾⣾⣿⣞⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⠋⣠⠟⠀⠀⠀⠀⠀⠀⠀⠀
        //⠀⠀⠀⠀⠀⠀⠀⠀⠀⠹⣿⣿⣿⠿⠿⣿⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⣾⣿⣿⣷⣦⣶⣦⣼⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣷⠈⠛⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀
        //⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠻⣿⣤⡖⠛⠶⠤⡀⠀⠀⠀⠀⠀⠀⠀⢰⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡿⠁⠙⣿⣿⠿⢻⣿⣿⡿⠋⢩⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
        //⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠙⠧⣤⣦⣤⣄⡀⠀⠀⠀⠀⠀⠘⢿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡇⠀⠀⠀⠘⣧⠀⠈⣹⡻⠇⢀⣿⡆⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
        //⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⣿⣿⣿⣿⣿⣤⣀⡀⠀⠀⠀⠀⠀⠀⠈⢽⣿⣿⣿⣿⣿⠋⠀⠀⠀⠀⠀⠀⠀⠀⠹⣷⣴⣿⣷⢲⣦⣤⡀⢀⡀⠀⠀⠀⠀⠀⠀
        //⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⢿⣿⣿⣿⣿⣿⣿⠟⠀⠀⠀⠀⠀⠀⠀⢸⣿⣿⣿⣿⣷⢀⡄⠀⠀⠀⠀⠀⠀⠀⠀⠈⠉⠂⠛⣆⣤⡜⣟⠋⠙⠂⠀⠀⠀⠀⠀
        //⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢹⣿⣿⣿⣿⠟⠀⠀⠀⠀⠀⠀⠀⠀⠘⣿⣿⣿⣿⠉⣿⠃⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣤⣾⣿⣿⣿⣿⣿⣆⠀⠰⠄⠀⠉⠀⠀
        //⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣸⣿⣿⡿⠃⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢹⣿⡿⠃⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢻⣿⠿⠿⣿⣿⣿⠇⠀⠀⢀⠀⠀⠀
        //⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣿⡿⠛⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⢻⡇⠀⠀⢀⣼⠗⠀⠀
        //⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⣿⠃⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⠁⠀⠀⠀
        //⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⠒⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀";

        public const string WorldMap = @"          . _..::__:  ,-""-""._       |]       ,     _,.__              
          _.___ _ _<_>`!(._`.`-.    /        _._     `_ ,_/  '  '-._.---.-.__ 
        .{     "" "" `-==,',._\{  \  / {)     / _ "">_,-' `                 /-/_ 
         \_.:--.       `._ )`^-. ""'      , [_/(                       __,/-'  
        '""'     \         ""    _L       |-_,--'                )     /. (|    
                 |           ,'         _)_.\\._<> {}              _,' /  '   
                 `.         /          [_/_'` `""(                <'}  )       
                  \\    .-. )          /   `-'""..' `:._          _)  '        
           `        \  (  `(          /         `:\  > \  ,-^.  /' '          
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

        //        private const int MapWidth = 64; // Ширина карты в символах
        //        private const int MapHeight = 18; // Высота карты в символах


        //        private struct Segment
        //        {
        //            public double MinLatitude; // Минимальная широта
        //            public double MaxLatitude; // Максимальная широта
        //            public double MinLongitude; // Минимальная долгота
        //            public double MaxLongitude; // Максимальная долгота

        //            public override string ToString()
        //            {
        //                return $"({MinLatitude} - {MaxLatitude}), ({MinLongitude} - {MaxLongitude})";
        //            }
        //        }

        //        // Массив сегментов
        //        private Segment[,] segments;
        //        public char[][] mapAsArray;


        //        public Map()
        //        {
        //            //InitializeSegments();
        //            //InitializeMapArray();
        //            Console.WriteLine(WorldMap);
        //        }



        //        public void AddCity(City city)
        //        {

        //        }


        //        private void InitializeMapArray()
        //        {
        //            string[] lines = World.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        //            char[][] charArray = new char[lines.Length][];

        //            for (int i = 0; i < lines.Length; i++)
        //            {
        //                charArray[i] = lines[i].ToCharArray();
        //            }
        //            mapAsArray = charArray;
        //        }

        //        private void InitializeSegments()
        //        {
        //            segments = new Segment[MapWidth, MapHeight];

        //            // Широта и долгота для карты
        //            double minLatitude = -90;
        //            double maxLatitude = 90;
        //            double minLongitude = -180;
        //            double maxLongitude = 180;


        //            Console.WriteLine(MapWidth + "    " + MapHeight);

        //            for (int y = 0; y < MapHeight - 1; y++)
        //            {
        //                for (int x = 0; x < MapWidth - 1; x++)
        //                {
        //                    segments[y, x] = new Segment
        //                    {
        //                        MinLatitude = minLatitude + y * (maxLatitude - minLatitude) / MapHeight,
        //                        MaxLatitude = minLatitude + (y + 1) * (maxLatitude - minLatitude) / MapHeight,
        //                        MinLongitude = minLongitude + x * (maxLongitude - minLongitude) / MapWidth,
        //                        MaxLongitude = minLongitude + (x + 1) * (maxLongitude - minLongitude) / MapWidth
        //                    };
        //                    Console.WriteLine(x + "  " + y + "   " + segments[y, x]);
        //                }
        //            }
        //        }

        //        public void PrintMap()
        //        {
        //            for (int i = 0; i < mapAsArray.Length; i++)
        //            {
        //                for (int j = 0; j < mapAsArray[i].Length; j++)
        //                {
        //                    Console.Write(mapAsArray[i][j]);
        //                }
        //                Console.WriteLine();
        //            }
        //        }













        //    // Добавление города на карту
        //    public string AddCityToMap(double latitude, double longitude)
        //    {
        //        // Находим сегмент, в который попадают координаты
        //        for (int y = 0; y < SegmentsY; y++)
        //        {
        //            for (int x = 0; x < SegmentsX; x++)
        //            {
        //                if (latitude >= segments[y, x].MinLatitude && latitude <= segments[y, x].MaxLatitude &&
        //                    longitude >= segments[y, x].MinLongitude && longitude <= segments[y, x].MaxLongitude)
        //                {
        //                    // Преобразуем координаты в позиции на карте
        //                    int localX = (int)((longitude - segments[y, x].MinLongitude) / (segments[y, x].MaxLongitude - segments[y, x].MinLongitude) * SegmentWidth);
        //                    int localY = (int)((segments[y, x].MaxLatitude - latitude) / (segments[y, x].MaxLatitude - segments[y, x].MinLatitude) * SegmentHeight);

        //                    // Глобальные координаты на карте
        //                    int globalX = x * SegmentWidth + localX;
        //                    int globalY = y * SegmentHeight + localY;
        //                    Console.WriteLine(x + "   " + y);
        //                    Console.ReadKey();

        //                    // Проверка на выход за границы карты
        //                    if (globalX < 0 || globalX >= MapWidth || globalY < 0 || globalY >= MapHeight)
        //                    {
        //                        throw new ArgumentException("Координаты выходят за пределы карты.");
        //                    }

        //                    // Вставляем звездочку на карту
        //                    string[] lines = World.Split(new[] { '\n' }, StringSplitOptions.None);
        //                    char[] lineChars = lines[globalY].ToCharArray();
        //                    lineChars[globalX] = '*';
        //                    lines[globalY] = new string(lineChars);

        //                    return string.Join("\n", lines);
        //                }
        //            }
        //        }

        //        throw new ArgumentException("Координаты не найдены в пределах карты.");
        //    }
        //}


    }
}
