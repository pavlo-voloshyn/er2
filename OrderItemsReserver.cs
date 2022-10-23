using System;
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
                string containerName = Environment.GetEnvironmentVariable("ContainerName");
                var blobClient = new BlobContainerClient(Connection, containerName);
                var blob = blobClient.GetBlobClient(Guid.NewGuid().ToString());
                await blob.UploadAsync(myQueueItem);
            } catch (Exception ex)
            {
                log.LogError(ex.Message, ex);
            }

        }
    }
}
