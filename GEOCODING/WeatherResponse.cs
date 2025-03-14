using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace GEOCODING
{
    public class WeatherResponse
    {
        public MainInfo Main { get; set; }
        public Weather[] Weather { get; set; }
        public WindInfo Wind { get; set; }
        public CloudsInfo Clouds { get; set; }

        public static WeatherResponse GetWeather(double latitude, double longitude)
        {
            const string apiKey = "3ce9d787aec683c60bb7da29967b7656";

            string requestUri = $"https://api.openweathermap.org/data/2.5/weather?lat={latitude.ToString(CultureInfo.InvariantCulture)}&lon={longitude.ToString(CultureInfo.InvariantCulture)}&appid={apiKey}";
            var response = GeocodingService.GetHttpClient.GetFromJsonAsync<WeatherResponse>(requestUri).GetAwaiter().GetResult();
            return response;
        }
        public static double KelvinToCelsius(double kelvin)
        {
            return kelvin - 273.15;
        }
    }

    public class MainInfo
    {
        public double Temp { get; set; } // Температура в Кельвинах
        public int Humidity { get; set; } // Влажность в процентах
    }

    public class Weather
    {
        public string Description { get; set; } // Описание погоды (например, "ясно", "дождь")
    }

    public class WindInfo
    {
        public double Speed { get; set; } // Скорость ветра в м/с
    }

    public class CloudsInfo
    {
        public int All { get; set; } // Облачность в процентах
    }
}
