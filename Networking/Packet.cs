using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;

namespace Networking
{
    public class Packet<T> where T : Stream
    {
        public T Stream { get; set; }

        public byte[] Buffer { get; set; }

        public Packet(int length)
        {
            Buffer = new byte[length];
        }
    }
}
