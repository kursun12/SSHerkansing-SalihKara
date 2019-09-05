using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using System.Collections.Specialized;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Queue;
using System.Text.RegularExpressions;

namespace BierRapportage
{
    public static class BeerFunction
    {
        [FunctionName("IsItTimeForBeer")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string cityName = req.Query["city"];
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            cityName = cityName ?? data?.cityName;

            if (string.IsNullOrWhiteSpace(cityName) || Regex.IsMatch(cityName, @"^[a-z]+$"))
            {
                return new BadRequestObjectResult("The input is not valid");
            }

            var storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
            var blobReference = $"{Guid.NewGuid().ToString()}.png";
            var queueClient = storageAccount.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference("myqueue-items");
            await queue.CreateIfNotExistsAsync();

            QueueMessage message = new QueueMessage(cityName, blobReference);
            string queueMessage = JsonConvert.SerializeObject(message);

            await queue.AddMessageAsync(new CloudQueueMessage(queueMessage));

            return new OkObjectResult($"Api call succesfull, There may be delay... if it does not work try again in a miunute, please download your results through this URL: https://functionsbier9cf7.blob.core.windows.net/blobs/" + blobReference);
        }
    }
}
