using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProcessRequest
{
    public class Message
    {
        [JsonPropertyName("startPosition")]
        public string startPosition
        {
            get;
            set;

        }

        [JsonPropertyName("endPosition")]
        public string endPosition
        {
            get;
            set;
        }
        [JsonPropertyName("operationId")]
        public string operationId
        {
            get; set;
        }
    }
}
