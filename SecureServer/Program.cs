using Networking;
using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace SecureServer
{
    class Program
    {
        static IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 13021);

        static Listener<ClientInstance> listener;

        static void Main(string[] args)
        {
            listener = new Listener<ClientInstance>(new X509Certificate2(@"..\..\..\..\certificates\DevelopServer.pfx", "test1234"));
            listener.clientDataReceivedEvent += OnClientDataReceived;
            listener.Start(endPoint);

            while (true) { Thread.Sleep(5); } // Keep server running
        }

        static void OnClientDataReceived(byte[] data)
        {
            Console.WriteLine($"Secured string from a client: {Encoding.UTF8.GetString(data)}");
        }
    }
}
