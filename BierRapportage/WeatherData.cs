using BierRapportage.Models;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace BierRapportage
{
    public class WeatherData
    {
        private readonly string URL = "http://api.openweathermap.org/data/2.5/weather";
        private readonly string key = "97652670aeaea38d487691029c6ec8e4";

        public WeatherData()
        {
        }

        public async Task<Weather> GetWeatherData(string city)
        {
            string apiQuery = "?q=" + city + "&appid=" + key;
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.GetAsync(apiQuery);

            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsAsync<Weather>().Result;
            }
            else
            {
                return null;
            }

        }
    }
}
