using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UnsecureClient
{
    class Program
    {
        static TcpClient client;
        static IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 13021);

        static void Main(string[] args)
        {
            client = new TcpClient();
            client.Connect(endPoint);

            while (true)
            {
                Console.ReadKey();
                Console.WriteLine("Sending data to server");
                client.GetStream().Write(Encoding.UTF8.GetBytes("{\"name\":\"test\", \"password\":\"GodKode123\"}"));
            }
        }
    }
}
