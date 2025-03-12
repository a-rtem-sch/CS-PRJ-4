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
    public static class GeocodingFillers
    {
        public static NominatimReverseResponse ReverseGeocode(double latitude, double longitude)
        {
            string requestUri = string.Format(
                CultureInfo.InvariantCulture,
                "https://nominatim.openstreetmap.org/reverse?lat={0}&lon={1}&format=json",
                latitude,
            longitude
            );

            var response = GeocodingService.GetHttpClient.GetFromJsonAsync<NominatimReverseResponse>(requestUri).GetAwaiter().GetResult();
            return response;
        }
    }
    public class NominatimReverseResponse
    {
        public string Display_Name { get; set; }
        public string Name { get; set; }
        public AddressInfo Address { get; set; }
        public string[] Boundingbox { get; set; }
    }

    public class AddressInfo
    {
        public string Amenity { get; set; }
        public string House_Number { get; set; }
        public string Road { get; set; }
        public string Suburb { get; set; }
        public string Borough { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Postcode { get; set; }
        public string Country { get; set; }
        public string Country_Code { get; set; }
    }
}
