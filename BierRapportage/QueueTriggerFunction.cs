using System;
using System.IO;
using System.Threading.Tasks;
using BierRapportage.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace BierRapportage.Functions
{
    public static class QueueTriggerFunction
    {
        [FunctionName("QueueTriggerFunction")]
        public static async Task RunAsync([QueueTrigger("myqueue-items", Connection = "AzureWebJobsStorage")]string QueueItem, ILogger log)
        {           
            try
            {
                log.LogInformation($"C# Queue trigger function processed");

                QueueMessage queueItem = Newtonsoft.Json.JsonConvert.DeserializeObject<QueueMessage>(QueueItem);
                WeatherData data = new WeatherData();
                Weather weather = await data.GetWeatherData(queueItem.cityName);
                Coordinates coordinates = weather.coord;
                Stream image;

                log.LogInformation($"C# Queue trigger function processed: Get beer advice, weather conditions are: {Math.Round(weather.main.Temp - 272.15, 2).ToString()} Celsius");

                string beerAdvice = GetBeerAdvice(weather);

                CloudBlockBlob blob = await GetStorageAccount(log, queueItem);

                MapData mapdata = new MapData();
                image= await mapdata.GetMapData(coordinates.lon, coordinates.lat);

                string Temp = "Temperature is : " + Math.Round(weather.main.Temp - 272.15, 2).ToString() + " Celcius";
                var ImageWithText = ImageHelper.AddTextToImage(image, (Temp, (10, 20)), (beerAdvice, (10, 50)));

                await blob.UploadFromStreamAsync(ImageWithText);
            }
            catch
            {
                log.LogError("something went wrong");
            }

        }
        private static async Task<CloudBlockBlob> GetStorageAccount(ILogger log, QueueMessage queueItem)
        {
            var storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
            //var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol = https; AccountName = functionsbier9cf7; AccountKey = 2WDyMhz / sXwGQBCMulFBDdims7lUVAZRalyMk6U7ZmkvpjvwpWrVRuCvsUDjZVKdyEut5xiQfnRMw1pnxC0Aiw ==; EndpointSuffix = core.windows.net");

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("blobs");

            await container.CreateIfNotExistsAsync();
            await container.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

            var blob = container.GetBlockBlobReference(queueItem.blobName);
            return blob;
        }

        private static string GetBeerAdvice(Weather weather)
        {
            double temp = weather.main.Temp;
            double tempCelcius = temp - 272.15;

            if (tempCelcius < 12)
            {
                return "too cold for beer!";
            }
            else if (tempCelcius > 12)
            {
                return "Time for beer!";
            }
            else
            {
                return "something went wrong, no information";
            }
        }
    }  
}
