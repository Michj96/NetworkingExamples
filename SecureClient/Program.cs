using System;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SecureClient
{
    class Program
    {
        static TcpClient client;
        static IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 13021);

        static X509Certificate2Collection certificates;
        static SslStream securedConnection;

        static void Main(string[] args)
        {
            client = new TcpClient();
            client.Connect(endPoint);

            certificates = new X509Certificate2Collection(
                    new X509Certificate2(@"..\..\..\..\certificates\DevelopClient.pfx", "test1234")
                );

            securedConnection = new SslStream(
                            client.GetStream(),
                            false,
                            new RemoteCertificateValidationCallback(ValidateServerCertificate),
                            new LocalCertificateSelectionCallback(CertificateSelectionCallback)
                        );
            securedConnection.AuthenticateAsClient("localhost", certificates, false);

            Console.WriteLine($"Server/Client auth status: {securedConnection.IsMutuallyAuthenticated}");

            while (true)
            {
                Console.ReadKey();
                Console.WriteLine("Sending data to server");
                securedConnection.Write(Encoding.UTF8.GetBytes("{\"name\":\"test\", \"password\":\"Better#¤Kode123\"}"));
            }
        }

        #region Certificate Callbacks
        private static bool ValidateServerCertificate(
           object sender,
             X509Certificate certificate,
             X509Chain chain,
             SslPolicyErrors sslPolicyErrors)
        {

            switch (sslPolicyErrors)
            {
                case SslPolicyErrors.None:
                    return true;
                default:
                    return false;
            }
        }

        private static X509Certificate CertificateSelectionCallback(
            object sender,
            string targetHost,
            X509CertificateCollection localCollection,
            X509Certificate remoteCertificate,
            string[] acceptableIssuers)
        {
            return localCollection[0];
        }
        #endregion
    }
}
