using Newtonsoft.Json;

namespace BierRapportage.Models
{
    public class Weather
    {
        [JsonProperty]
        public string name { get; set; }
        [JsonProperty]
        public Coordinates coord { get; set; }
        [JsonProperty]
        public Temperature main { get; set; }
        [JsonProperty]
        public Wind wind { get; set; }
    }

    public class Temperature
    {
        [JsonProperty]
        public double Temp { get; set; }
        [JsonProperty]
        public int Pressure { get; set; }
        [JsonProperty]
        public int Humidity { get; set; }
        [JsonProperty]
        public double Temp_min { get; set; }
        [JsonProperty]
        public double Temp_max { get; set; }
    }

    public class Coordinates
    {
        public string lon { get; set; }

        public string lat { get; set; }
    }
}