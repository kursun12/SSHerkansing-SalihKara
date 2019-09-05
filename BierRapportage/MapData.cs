using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace BierRapportage
{
    public class MapData
    {
        private readonly string url = "https://atlas.microsoft.com/map/static/png";
        private readonly string key = "RQgNHxNSRSrTkUnRIb5Tgka1rcvCLDWD7ijNJay3mNc";

        public MapData()
        {
        }

        public async Task<Stream> GetMapData(string Longitutde, string Latitude)
        {
            HttpClient client = new HttpClient();
            string URL = url + "?subscription-key=" + key + "&api-version=1.0&center=" + Longitutde + "," + Latitude + "&language=nl-NL";
            HttpResponseMessage response = await client.GetAsync(URL);

            Stream map;
            if (response.IsSuccessStatusCode)
            {
                map = await response.Content.ReadAsStreamAsync();
                return map;
            }
            else
            {
                return null;
            }
            
        }
    }
}
