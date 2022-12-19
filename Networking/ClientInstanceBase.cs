using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;

namespace Networking
{
    public abstract class ClientInstanceBase
    {
        public delegate void clientDataReceived(byte[] data);
        public event clientDataReceived DataReceivedEvent;

        public delegate void clientDisconnect(object instance);
        public event clientDisconnect DisconnectEvent;

        private TcpClient _clientConnection;
        private SslStream _secureClientConnection;

        private AsyncCallback _callback;

        public void Initialize(TcpClient connection)
        {
            _clientConnection = connection;
            _callback = new AsyncCallback(DataReceived<NetworkStream>);

            Data<NetworkStream>();
        }

        public void Initialize(SslStream connection)
        {
            _secureClientConnection = connection;
            _callback = new AsyncCallback(DataReceived<SslStream>);

            Data<SslStream>();
        }

        private void Data<T>()
        {
            if (typeof(T) == typeof(NetworkStream))
            {
                Networking.Packet<NetworkStream> packet = new Networking.Packet<NetworkStream>(16384);
                packet.Stream = _clientConnection.GetStream();
                _clientConnection.GetStream().BeginRead(packet.Buffer, 0, packet.Buffer.Length, _callback, packet);
            }
            else if(typeof(T) == typeof(SslStream))
            {
                Networking.Packet<SslStream> packet = new Networking.Packet<SslStream>(16384);
                packet.Stream = _secureClientConnection;
                _secureClientConnection.BeginRead(packet.Buffer, 0, packet.Buffer.Length, _callback, packet);
            }
        }

        private void DataReceived<T>(IAsyncResult iaResult) where T : Stream
        {
            Networking.Packet<T> packet = (Networking.Packet<T>)iaResult.AsyncState;

            try
            {
                packet.Stream.EndRead(iaResult);
            }
            catch
            {
                DisconnectEvent?.Invoke(this);
                return;
            }

            DataInvoking(packet.Buffer);
            Data<T>();
        }

        private void DataInvoking(byte[] data)
        {
            DataReceivedEvent?.Invoke(data);
            DataHandled(data);
        }
        public abstract void DataHandled(byte[] data);

        public void SendData(byte[] data)
        {
            if (data.Length > 16384)
            {
                throw new ArgumentOutOfRangeException("Data length can not exceed 16384 bytes.");
            }
            _clientConnection.GetStream().WriteAsync(data, 0, data.Length);
        }
    }
}
