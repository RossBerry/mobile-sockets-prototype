using System;
using System.Collections.Generic;
using System.Text;
using SocketHelpers.Discovery;

namespace SocketsPrototype.Services
{
    class DiscoverPayload : IDiscoveryPayload
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
