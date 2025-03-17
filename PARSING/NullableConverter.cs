using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PARSING
{
    //public class NullableConverter<T> : DefaultTypeConverter where T : struct
    //{
    //    public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
    //    {
    //        if (string.IsNullOrWhiteSpace(text))
    //        {
    //            return null; // Возвращаем null для пустых значений
    //        }

    //        // Пытаемся преобразовать строку в тип T
    //        if (typeof(T) == typeof(int) && int.TryParse(text, out int intValue))
    //        {
    //            return intValue;
    //        }
    //        if (typeof(T) == typeof(ulong) && ulong.TryParse(text, out ulong ulongValue))
    //        {
    //            return ulongValue;
    //        }
    //        if (typeof(T) == typeof(double) && double.TryParse(text, CultureInfo.InvariantCulture, out double doubleValue))
    //        {
    //            return doubleValue;
    //        }

    //        // Если преобразование не удалось, возвращаем null
    //        return null;
    //    }
    //}
}
