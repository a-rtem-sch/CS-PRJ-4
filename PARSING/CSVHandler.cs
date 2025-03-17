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
            List<BadRecord> badRecords = [];
            List<City> cities = [];

            CsvConfiguration config = new (CultureInfo.InvariantCulture)
            {
                BadDataFound = context => badRecords.Add(new BadRecord
                {
                    RawRecord = context.RawRecord,
                    Field = context.Field,
                    ErrorMessage = context.RawRecord ?? "Unknown error"
                }),
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

            using StreamReader reader = new(filePath);
            using CsvReader csv = new(reader, config);

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
        public static void ExportCitiesToCsv(CityCollection cityCollection, string filePath)
        {
            List<City> cities = cityCollection.Cities;

            using StreamWriter writer = new(filePath);
            using CsvWriter csv = new(writer, CultureInfo.InvariantCulture);

            csv.WriteRecords(cities);
        }
    }


}
