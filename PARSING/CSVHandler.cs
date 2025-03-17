using CITIES;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;


namespace PARSING
{
    public class CSVHandler
    {
        public static (List<City> Cities, List<BadRecord> BadRecords) ImportCitiesFromCsv(string filePath)
        {
            var badRecords = new List<BadRecord>();
            var cities = new List<City>();

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                BadDataFound = context =>
                {
                    badRecords.Add(new BadRecord
                    {
                        RawRecord = context.RawRecord,
                        Field = context.Field,
                        ErrorMessage = context.RawRecord ?? "Unknown error"
                    });
                },
                MissingFieldFound = null, // Игнорируем отсутствующие поля
                HeaderValidated = null,   // Игнорируем ошибки валидации заголовков
                IgnoreBlankLines = true,
                PrepareHeaderForMatch = args => args.Header.ToLower(), // Приводим заголовки к нижнему регистру
                ReadingExceptionOccurred = exception =>
                {
                    // Логируем ошибку и продолжаем парсинг
                    badRecords.Add(new BadRecord
                    {
                        RawRecord = GeneralParsing.ExtractRawRecord(exception.Exception.ToString()),

                    });
                    return false; // Продолжаем парсинг после ошибки
                }
            };

            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, config);

            try
            {
                cities = csv.GetRecords<City>().ToList();
            }
            catch (CsvHelperException ex)
            {
                badRecords.Add(new BadRecord
                {
                    RawRecord = ex.Context?.Parser?.RawRecord ?? "N/A",
                    Field = ex.Context?.Reader?.CurrentIndex.ToString() ?? "N/A",
                    ErrorMessage = ex.Message
                });
            }

            return (cities, badRecords);
        }
        public static void ExportCitiesToCsv(List<City> cities, string filePath)
        {
            using var writer = new StreamWriter(filePath);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            // Регистрируем кастомный маппинг
            csv.Context.RegisterClassMap<CityMap>();

            // Записываем данные в CSV
            csv.WriteRecords(cities);
        }
    }

    public sealed class CityMap : ClassMap<City>
    {
        public CityMap()
        {
            // Указываем, какие поля включать в CSV
            Map(m => m.Name).Name("Name");
            Map(m => m.Country).Name("Country");
            Map(m => m.Population).Name("Population");
            Map(m => m.Latitude).Name("Latitude");
            Map(m => m.Longitude).Name("Longitude");
        }
    }


}
