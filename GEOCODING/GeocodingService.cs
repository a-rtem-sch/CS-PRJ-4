using System.Net.Http.Json;

namespace GEOCODING
{
    public  class GeocodingService
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public static async Task<(double Latitude, double Longitude)?> GeocodeAsync(string cityName)
        {
            string requestUri = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(cityName)}&format=json&limit=1";
            var response = await _httpClient.GetFromJsonAsync<NominatimResponse[]>(requestUri);

            if (response != null && response.Length > 0)
            {
                return (double.Parse(response[0].Lat), double.Parse(response[0].Lon));
            }

            return null; // Координаты не найдены
        }

        public static async Task<string> ReverseGeocodeAsync(double latitude, double longitude)
        {
            string requestUri = $"https://nominatim.openstreetmap.org/reverse?lat={latitude}&lon={longitude}&format=json";
            var response = await _httpClient.GetFromJsonAsync<NominatimReverseResponse>(requestUri);

            return response?.Display_Name ?? "Информация не найдена";
        }

        // Класс для десериализации ответа Nominatim (обратное геокодирование)
        private class NominatimReverseResponse
        {
            public string Display_Name { get; set; } // Адрес и дополнительная информация
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
