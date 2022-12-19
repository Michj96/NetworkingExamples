using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Networking
{
    public class Listener<T> where T : ClientInstanceBase
    {
        private List<T> _clients;

        private TcpListener _listener;
        private X509Certificate2 _certificate;

        public delegate void clientDataReceived(byte[] data);
        public event clientDataReceived clientDataReceivedEvent;

        public Listener()
        {
            _clients = new List<T>();
        }

        public Listener(X509Certificate2 certificate)
        {
            _clients = new List<T>();
            _certificate = certificate;
        }

        public void Start(IPEndPoint endpoint)
        {
            _listener = new TcpListener(endpoint);
            _listener.Start();
            _listener.BeginAcceptTcpClient(new AsyncCallback(ProcessClient), null);
        }

        private void ProcessClient(IAsyncResult iaResult)
        {
            TcpClient client = _listener.EndAcceptTcpClient(iaResult);
            _listener.BeginAcceptTcpClient(new AsyncCallback(ProcessClient), null);

            T cInstance = (T)Activator.CreateInstance(typeof(T));
            if (_certificate != null)
            {
                // Connection secured with certificate
                SslStream secureClient = new SslStream(client.GetStream(), false, ValidateServerCertificate, CertificateSelectionCallback);
                secureClient.AuthenticateAsServer(_certificate, true, false);

                // ToDo: SSL Timeout infinite, not really important in this context.

                cInstance.Initialize(secureClient);
            }
            else
            {
                // Connection unsecured
                cInstance.Initialize(client);
            }

            cInstance.DataReceivedEvent += OnClientData;
            cInstance.DisconnectEvent += OnClientDisconnect;

            lock(_clients)
            {
                _clients.Add(cInstance);
            }
        }

        private void OnClientData(byte[] data)
        {
            clientDataReceivedEvent?.Invoke(data);
        }

        private void OnClientDisconnect(object instance)
        {
            lock (_clients)
            {
                _clients.Remove((T)instance);
            }
        }

        #region Certificate Callbacks
        // ToDo: Mutual authentication WIP
        private static bool ValidateServerCertificate(
           object sender,
             X509Certificate certificate,
             X509Chain chain,
             SslPolicyErrors sslPolicyErrors)
        {
            Console.WriteLine(sslPolicyErrors);

            Console.WriteLine("Client Sender: " + sender.ToString());
            Console.WriteLine("Server [subject] and CA [issuer] certificate params : " +
            certificate.ToString());

            switch (sslPolicyErrors)
            {
                case SslPolicyErrors.None:
                    return true;
                default:
                    return false;
            }
        }

        private X509Certificate CertificateSelectionCallback(
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
