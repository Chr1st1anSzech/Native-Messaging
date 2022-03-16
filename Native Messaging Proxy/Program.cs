using log4net;
using log4net.Config;
using NativeMessaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Native_Messaging_Proxy
{
    class Program
    {
        private static readonly ILog s_log = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

        private static Host s_host = null!;

        private static readonly string[] s_allowedOrigins = new string[]
            {
                "chrome-extension://idmbccigjgpdfbaafcdinielklighfbb/"
            };

        private static readonly string s_description = "Christian Szech Native Messaging Host";

        private static readonly Regex s_keyValuePairRegex = new(@"\s*\S+\s*=\s*\S+\s*");

        static void Main(string[] args)
        {
            ConfigLogging();

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
            TcpListener listener = null;
            try
            {
                int port = 1234;
                string ip = "127.0.0.1";
                listener = new(IPAddress.Parse(ip), port);

                s_log.Info($"Start listening to {ip}:{port}.");
                listener.Start();

                while (true)
                {
                    s_log.Debug("Waiting for Connection.");

                    using TcpClient client = listener.AcceptTcpClient();
                    using NetworkStream stream = client.GetStream();
                    using StreamReader reader = new(stream, Encoding.ASCII);

                    s_log.Info("Client accepted.");

                    string? inputLine = string.Empty;
                    while ( (inputLine = reader.ReadLine() ) != null)
                    {
                        s_log.Debug($"Read line \"{inputLine}\".");
                        SendInputToExtension(inputLine);
                    }
                }
            }
            catch (SocketException e)
            {
                s_log.Error($"SocketException: {e.Message}.");
            }
            catch (Exception ex)
            {
                s_log.Error($"Error while reading input. Message: {ex.Message}.");
            }
            finally
            {
                listener?.Stop();
                ListenInput();
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

                s_log.Info($"Send message to browser. Operation=\"{operation}\", Value=\"{value}\"");

                s_host.SendMessage(obj);
            }
        }

        private static void ConfigLogging()
        {
            string appPath = Util.GetApplicationRoot();
            var logFile = Path.Combine(appPath, "log4net.xml");
            if (Util.IsDirectoryWritable(appPath))
            {
                XmlConfigurator.Configure(new FileInfo(logFile));
            }
        }
    }
}