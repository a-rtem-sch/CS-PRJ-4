using System.Globalization;
using System.Net.Http.Json;

namespace GEOCODING
{
    /// <summary>
    /// Класс с методами для работы с асинхронным геокодингом
    /// </summary>
    public static class GeocodingService
    {
        public static HttpClient GetHttpClient { get; } = new();

        static GeocodingService()
        {
            GetHttpClient.DefaultRequestHeaders.UserAgent.ParseAdd("CSPRJ4/1.0");
        }

        /// <summary>
        /// Асинхроннный геокодинг по названию
        /// </summary>
        /// <param name="query">Запрос из имени города и страны</param>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        public static async Task<(double Latitude, double Longitude)?> GeocodeAsync(string query)
        {
            string requestUri = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(query)}&format=json&limit=1";
            NominatimResponse[]? response = await GetHttpClient.GetFromJsonAsync<NominatimResponse[]>(requestUri);

            return response != null && response.Length > 0
                ? double.TryParse(response[0].Lat, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double lat) &&
                    double.TryParse(response[0].Lon, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double lon)
                    ? ((double Latitude, double Longitude)?)(lat, lon)
                    : throw new FormatException("Invalid latitude or longitude format.")
                : null;
        }

        /// <summary>
        /// Обратный геокод по координатам
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public static async Task<string> ReverseGeocodeAsync(double latitude, double longitude)
        {

            string requestUri = string.Format(
                CultureInfo.InvariantCulture,
                "https://nominatim.openstreetmap.org/reverse?lat={0}&lon={1}&format=json",
                latitude,
                longitude
            );
            NominatimReverseResponse? response = await GetHttpClient.GetFromJsonAsync<NominatimReverseResponse>(requestUri);

            return response?.Display_Name ?? "Информация не найдена";
        }


        // Вложенный (nested) класс, который используется только в GeocodingService
        private class NominatimResponse
        {
            public string Lat { get; set; }
            public string Lon { get; set; }
            public string Display_Name { get; set; }
        }

    }
    
}
