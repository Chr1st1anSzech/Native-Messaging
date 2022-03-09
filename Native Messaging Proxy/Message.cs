using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Native_Messaging_Proxy
{
    internal class Message
    {
        [JsonProperty("Operation")]
        public string? Operation { get; set; }

        [JsonProperty("Value")]
        public string? Value { get; set; }

        public Message(string operation, string value)
        {
            if(string.IsNullOrWhiteSpace(operation) || string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException($"Message not created. {nameof(operation)} or {nameof(operation)} is invalid.");
            }
            Value = value;
            Operation = operation;
        }
    }
}
