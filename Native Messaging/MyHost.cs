using NativeMessaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Native_Messaging
{
    public class MyHost : Host
    {
        private const bool SendConfirmationReceipt = true;

        public override string Hostname
        {
            get { return "com.my_company.my_application"; }
        }

        public MyHost() : base(SendConfirmationReceipt)
        {

        }

        public class OpenUrl
        {
            [JsonProperty("Url")]
            public string Url { get; set; } = "";
        }

        protected override void ProcessReceivedMessage(JObject data)
        {
            OpenUrl openUrl = new() { Url = "https://www.drwindows.de/news/" };
            JObject? obj = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(openUrl));
            SendMessage(obj);
        }
    }
}