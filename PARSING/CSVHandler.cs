using CITIES;
using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                        Field = context.Field
                    });
                },
                MissingFieldFound = null, // Игнорируем отсутствующие поля
                HeaderValidated = null,   // Игнорируем ошибки валидации заголовков
                IgnoreBlankLines = true,

                PrepareHeaderForMatch = args => args.Header.ToLower(), // Приводим заголовки к нижнему регистру
            };

            string absolutePath = Path.GetFullPath(filePath);
            using var reader = new StreamReader(absolutePath);
            using var csv = new CsvReader(reader, config);

            // Регистрируем кастомный конвертер для всех nullable-типов
            csv.Context.TypeConverterCache.AddConverter<ulong?>(new NullableConverter<ulong>());
            csv.Context.TypeConverterCache.AddConverter<int?>(new NullableConverter<int>());
            csv.Context.TypeConverterCache.AddConverter<double?>(new NullableConverter<double>());

            try
            {
                cities = csv.GetRecords<City>().ToList();
            }
            catch (CsvHelperException ex)
            {
                badRecords.Add(new BadRecord
                {
                    RawRecord = ex.Context.Parser.RawRecord,
                    Field = ex.Message
                });
            }

            return (cities, badRecords);
        }
        public static void ExportCitiesToCsv(List<City> cities, string filePath)
        {
            using var writer = new StreamWriter(filePath);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteRecords(cities);
        }
    }
    public class BadRecord
    {
        public string RawRecord { get; set; }
        public string Field { get; set; }
    }
}
