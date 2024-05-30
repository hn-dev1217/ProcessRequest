using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Cosmos;
using ProcessRequest;

namespace CalculateNoOfMoves
{
    public class CalcNoOfMovesFunction
    {

        [Function("CalcNoOfMovesFunction")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "knightpath")] HttpRequest req)
        {
            //log.LogInformation("C# HTTP trigger function processed a request.");
            var operation_id = req.Query["operationId"];

            if (string.IsNullOrEmpty(operation_id))
            {
                return new BadRequestObjectResult("operation_id is required");
            }

            CosmosClient client = new(
accountEndpoint: $"https://knightpathdb.documents.azure.com:443/",
authKeyOrResourceToken: "Wwc4t333Bwc0G38dfg20MDNs4cv5Xzwkfu6wXoc7FpRbPkT4RRWr5sTxZs7yZk3XP8z22fuzBZM1ACDbZN8EOA=="
);

            Database database2 = await client.CreateDatabaseIfNotExistsAsync(
    id: "knightpath"
);

            Container container2 = await database2.CreateContainerIfNotExistsAsync(
    id: "knightpath-storage",
    partitionKeyPath: "/operationId",
    throughput: 400
);
            Response readItem = await container2.ReadItemAsync<Response>(
    id: operation_id,
    partitionKey: new PartitionKey(operation_id)
);

            var response = JsonConvert.SerializeObject(readItem);

            return new OkObjectResult(response);
        }
    }
}
