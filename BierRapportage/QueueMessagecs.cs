using Newtonsoft.Json;

namespace BierRapportage
{
    public class QueueMessage
    {
        [JsonProperty]
        public string cityName { get; set; }
        [JsonProperty]
        public string blobName { get; set; }

        public QueueMessage(string cityName, string blobName)
        {
            this.cityName = cityName;
            this.blobName = blobName;
        }

    }
}
