using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace dotMorten.WeatherGov
{
    public class Client
    {
        // 
        // PointsForecast myDeserializedClass = JsonConvert.DeserializeObject<PointsForecast>(myJsonResponse); 

        // https://api.weather.gov/gridpoints/SGX/65,71/forecast
        public static async Task<Client> CreateWeatherClientAsync(double latitude, double longitude)
        {
            var c = new Client(latitude, longitude);
            await c.Initialize().ConfigureAwait(false);
            return c;
        }
        private double _latitude;
        private double _longitude;
        private Client(double latitude, double longitude)
        {
            _latitude = latitude;
            _longitude = longitude;
        }
        private Points.Root forecastInfo;
        private async Task Initialize()
        {
            using HttpClient http = new HttpClient();
            var url = $"https://api.weather.gov/points/{_latitude},{_longitude}";
            try
            {
                http.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("StatusDisplay", "1.0"));
                var json = await http.GetStringAsync(url).ConfigureAwait(false);
                forecastInfo = JsonConvert.DeserializeObject<Points.Root>(json);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<Gridpoints.Root> GetForecastAsync()
        {
            using HttpClient http = new HttpClient();
            http.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("StatusDisplay", "1.0"));
            var json = await http.GetStringAsync(forecastInfo.Properties.ForecastGridData).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<Gridpoints.Root>(json);
        }
        public async Task<Gridpoints.Root> GetDailyForecastAsync()
        {
            using HttpClient http = new HttpClient();
            http.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("StatusDisplay", "1.0"));
            var json = await http.GetStringAsync(forecastInfo.Properties.Forecast).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<Gridpoints.Root>(json);
        }
        public async Task<Gridpoints.Root> GetHourlyForecastAsync()
        {
            using HttpClient http = new HttpClient();
            http.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("StatusDisplay", "1.0"));
            var json = await http.GetStringAsync(forecastInfo.Properties.ForecastHourly).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<Gridpoints.Root>(json);
        }
        // Forecast myDeserializedClass = JsonConvert.DeserializeObject<Forecast>(myJsonResponse); 
    }
}