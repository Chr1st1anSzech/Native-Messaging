using NativeMessaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace Native_Messaging_Proxy
{
    class Program
    {
        static Host Host = null!;

        readonly static string[] AllowedOrigins = new string[]
            {
                "chrome-extension://idmbccigjgpdfbaafcdinielklighfbb/"
            };

        readonly static string Description = "Christian Szech Native Messaging Host";

        private static void ListenInput()
        {
            int port = 1234;
            TcpListener listener = new(IPAddress.Parse("127.0.0.1"), port);
            listener.Start();

            using TcpClient client = listener.AcceptTcpClient();
            using NetworkStream stream = client.GetStream();
            using StreamReader reader = new(stream, Encoding.ASCII);

            while (true)
            {
                try
                {
                    string? inputLine = reader.ReadLine();
                    if (inputLine != null && Regex.IsMatch(inputLine, @"\S+\s*=\s*\S+"))
                    {
                        string[] splitArray = inputLine.Split('=', 2);
                        string operation = splitArray[0].Trim().ToLower();
                        string value = splitArray[1].Trim();
                        Input input = new(operation, value);
                        JObject? obj = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(input));
                        Host.SendMessage(obj);
                    }
                }
                catch (Exception ex)
                {
                    // Todo: Exception Handling
                }
            }

        }

        public class Input
        {
            [JsonProperty("Operation")]
            public string? Operation { get; set; }

            [JsonProperty("Value")]
            public string? Value { get; set; }

            public Input(string operation, string value)
            {
                Value = value;
                Operation = operation;
            }
        }

        static void Main(string[] args)
        {
            Log.Active = true;

            Host = new MyHost();
            Host.SupportedBrowsers.Add(ChromiumBrowser.GoogleChrome);
            Host.SupportedBrowsers.Add(ChromiumBrowser.MicrosoftEdge);

            if (args.Contains("--register"))
            {
                Host.GenerateManifest(Description, AllowedOrigins);
                Host.Register();
            }
            else if (args.Contains("--unregister"))
            {
                Host.Unregister();
            }
            else
            {
                ListenInput();
            }
        }
    }
}