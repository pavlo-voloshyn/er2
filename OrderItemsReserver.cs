using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace OrderReserver
{
    public class OrderItemsReserver
    {
        [FunctionName("OrderItemsReserver")]
        public async Task Run([ServiceBusTrigger("orders", Connection = "ServiceBusConnection")]string myQueueItem, ILogger log)
        {
            try
            {
                log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");

                string Connection = Environment.GetEnvironmentVariable("Storage");
                log.LogInformation(Connection);
                string containerName = Environment.GetEnvironmentVariable("ContainerName");
                log.LogInformation(containerName);
                var blobClient = new BlobContainerClient(Connection, containerName);
                var blob = blobClient.GetBlobClient(Guid.NewGuid().ToString() + ".json");

                if (Convert.ToBoolean(Environment.GetEnvironmentVariable("ThrowsError")))
                {
                    await blob.UploadAsync(myQueueItem);
                }
                else
                {
                    await blob.UploadAsync(new BinaryData(myQueueItem));
                }
            }
            catch (Exception _)
            {
                await new HttpClient().PostAsync(
                    Environment.GetEnvironmentVariable("LogicAppEmailSenderUrl"), 
                    new StringContent(myQueueItem, Encoding.UTF8, "application/json"));
            }
        }
    }
}
