using Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureServer
{
    class ClientInstance : ClientInstanceBase
    {
        public override void DataHandled(byte[] data)
        {
            // Not used in this context.
            // Use this to process individual ClientInstance data
        }
    }
}
