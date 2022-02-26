using System.Net.Sockets;
using System.Text;

namespace Native_Messaging_Companion
{
    class Program
    {
       
        static void Main(string[] args)
        {
            using var client = new TcpClient();

            var hostname = "127.0.0.1";
            try
            {
                client.Connect(hostname, 1234);
                using NetworkStream networkStream = client.GetStream();
                networkStream.ReadTimeout = 2000;

                using var writer = new StreamWriter(networkStream);

                using var reader = new StreamReader(networkStream, Encoding.UTF8);

                while (true)
                {
                    Console.WriteLine("Nachricht:");
                    string? message = Console.ReadLine();
                    if( !string.IsNullOrWhiteSpace(message))
                    {
                        message += "\r\n";
                        byte[] bytes = Encoding.UTF8.GetBytes(message);
                        networkStream.Write(bytes, 0, bytes.Length);
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Port nicht geöffnet");
            }
        }
    }
}