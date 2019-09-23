using System;
using System.Collections.Generic;
using System.Text;

namespace SocketsPrototype.Services
{
    class DiscoverPayload
    {
        public string RemoteAddress { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int RemotePort { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public DiscoverPayload(string remoteAddress, int remotePort)
        {
            RemoteAddress = remoteAddress;
            RemotePort = remotePort;
        }

    }
}
