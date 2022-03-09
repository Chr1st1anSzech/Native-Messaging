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
        private static Host s_host = null!;

        private static readonly string[] s_allowedOrigins = new string[]
            {
                "chrome-extension://idmbccigjgpdfbaafcdinielklighfbb/"
            };

        private static readonly string s_description = "Christian Szech Native Messaging Host";

        private static readonly Regex s_keyValuePairRegex = new(@"\s*\S+\s*=\s*\S+\s*");

        static void Main(string[] args)
        {
            Log.Active = true;

            s_host = new MyHost();
            s_host.SupportedBrowsers.Add(ChromiumBrowser.GoogleChrome);
            s_host.SupportedBrowsers.Add(ChromiumBrowser.MicrosoftEdge);

            if (args.Contains("--register"))
            {
                s_host.GenerateManifest(s_description, s_allowedOrigins);
                s_host.Register();
            }
            else if (args.Contains("--unregister"))
            {
                s_host.Unregister();
            }
            else
            {
                ListenInput();
            }
        }

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
                    SendInputToExtension(inputLine);
                }
                catch (Exception ex)
                {
                    // Todo: Exception Handling
                }
            }

        }

        private static void SendInputToExtension(string? inputLine)
        {
            if (inputLine != null && s_keyValuePairRegex.IsMatch(inputLine))
            {
                string[] splitArray = inputLine.Split('=', 2);
                string operation = splitArray[0].Trim().ToLower();
                string value = splitArray[1].Trim();
                Message msg = new(operation, value);
                JObject? obj = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(msg));
                s_host.SendMessage(obj);
            }
        }
    }
}