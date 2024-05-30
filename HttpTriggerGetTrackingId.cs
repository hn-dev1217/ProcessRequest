using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using Azure.Messaging.ServiceBus;
using Azure.Identity;

using ProcessRequest;
using Microsoft.Azure.Functions.Worker;

namespace test.venteur
{
    public static class HttpTriggerGetTrackingId
    {
        [Function("HttpTriggerGetTrackingId")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "knightpath")] HttpRequest req)
        {
            string start_pos = req.Query["source"];
            string end_pos = req.Query["target"];

            if(string.IsNullOrEmpty(start_pos) || string.IsNullOrEmpty(end_pos))
            {
                return new BadRequestObjectResult("source and target are required");
            }

            var operation_id = Guid.NewGuid();

            // name of Service Bus queue
            // the client that owns the connection and can be used to create senders and receivers
            ServiceBusClient client;

            // the sender used to publish messages to the queue
            ServiceBusSender sender;

            // number of messages to be sent to the queue
            const int numOfMessages = 1;

            Message serviceBusMsg = new Message {startPosition = start_pos, endPosition = end_pos, operationId = operation_id.ToString()};

            //serializing the message based on the input received.
            var serializedMsg =  JsonConvert.SerializeObject(serviceBusMsg);
          
            client = new ServiceBusClient(
                 "Endpoint=sb://knightpath.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=8ApUzEU2D77yUDeOb26rn8d6mNxDstRqh+ASbIvkOLA=");
            sender = client.CreateSender("knightpathqueue");
            // create a batch 
            using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

            for (int i = 1; i <= numOfMessages; i++)
            {
                // try adding a message to the batch
                if (!messageBatch.TryAddMessage(new ServiceBusMessage(serializedMsg)))
                {
                    // if it is too large for the batch
                    throw new Exception($"The message {i} is too large to fit in the batch.");
                }
            }

            try
            {
                // Use the producer client to send the batch of messages to the Service Bus queue
                await sender.SendMessagesAsync(messageBatch);
                Console.WriteLine($"A batch of {numOfMessages} messages has been published to the queue.");
            }
            finally
            {
                // Calling DisposeAsync on client types is required to ensure that network
                // resources and other unmanaged objects are properly cleaned up.
                await sender.DisposeAsync();
                await client.DisposeAsync();
            }

            var response = new TrackingResponse {operationId=operation_id.ToString()};

            return new OkObjectResult(response);
        }
    }
}
