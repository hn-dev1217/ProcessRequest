using System;
using System.Drawing.Text;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos;
using static System.Net.WebRequestMethods;
using System.Runtime.Intrinsics.Arm;

namespace ProcessRequest
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [Function(nameof(Function1))]
        public async Task Run(
            [ServiceBusTrigger("knightpathqueue", Connection = "ServiceBus")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Body: {body}", message.Body);
            _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

            //TODO: algorithm for processsing shortest path

            var serviceBusMsgDes = JsonConvert.DeserializeObject<Message>(message.Body.ToString());
            _logger.LogInformation("Deserialized operation Id: {operationId}", serviceBusMsgDes.operationId);

            //creating database
            CosmosClient client = new(accountEndpoint: $"https://knightpathdb.documents.azure.com:443/",
                authKeyOrResourceToken: "Wwc4t333Bwc0G38dfg20MDNs4cv5Xzwkfu6wXoc7FpRbPkT4RRWr5sTxZs7yZk3XP8z22fuzBZM1ACDbZN8EOA==");

            Database database2 = await client.CreateDatabaseIfNotExistsAsync(id: "knightpath");

            Container container2 = await database2.CreateContainerIfNotExistsAsync(id: "knightpath-storage", partitionKeyPath: "/operationId", throughput: 400);

            //calculating shortest path based on the start position and end position.
            var shortPath = FindShortestPath.shortestPath(serviceBusMsgDes.startPosition, serviceBusMsgDes.endPosition);
            var x ="";

            foreach(var y in shortPath.Item1)
            {
                x = x + ":" + y;
                x = x.Trim(':');
            }

            Response resp = new Response { id = serviceBusMsgDes.operationId.ToString(), ending = serviceBusMsgDes.endPosition, starting = serviceBusMsgDes.startPosition, numberOfMoves = shortPath.Item2, shortestPath = x, operationId = serviceBusMsgDes.operationId.ToString() };

            Response createdItem = await container2.CreateItemAsync<Response>(item: resp, partitionKey: new PartitionKey(resp.operationId));
            // Complete the message
            await messageActions.CompleteMessageAsync(message);
        }

        public static (int, int) KnightPositionsIndex(string position)
        {
            var column = position[0];
            var row = position[1];

            var x = column - 'a';
            var y = int.Parse(row.ToString()) - 1;

            return (x,y);
        }

        public static string IndexToKnightPositions(int row, int col)
        {
            if (row < 0 || row > 7 || col < 0 || col > 7)
            {
                throw new Exception("Indices out of bounds");
            }
            char colC = (char)('a' + col);
            char rowR = (char)('1' + row);

            return $"{colC}{rowR}";

        }
    }
}
