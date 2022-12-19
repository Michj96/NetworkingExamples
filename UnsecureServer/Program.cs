using Networking;
using System;
using System.Net;
using System.Text;
using System.Threading;

namespace UnsecureServer
{
    class Program
    {
        static IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 13021);

        static Listener<ClientInstance> listener;

        static void Main(string[] args)
        {
            listener = new Listener<ClientInstance>();
            listener.clientDataReceivedEvent += OnClientDataReceived;
            listener.Start(endPoint);

            while (true) { Thread.Sleep(5); } // Keep server running
        }

        static void OnClientDataReceived(byte[] data)
        {
            Console.WriteLine($"String from a client: {Encoding.UTF8.GetString(data)}");
        }
    }
}
