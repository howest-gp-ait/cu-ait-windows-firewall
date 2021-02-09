using System;
using System.Collections.Generic;
using System.Text;
using Ait.WindowsFirewall.Core.Entities;

namespace Ait.WindowsFirewall.Core.Services
{
    public class ProtocolService
    {
        private List<Protocol> protocols;

        public List<Protocol> Protocols
        {
            get { return protocols; }
        }
        public ProtocolService()
        {
            protocols = new List<Protocol>();
            protocols.Add(new Protocol("ICMP", 1));
            protocols.Add(new Protocol("IGMP", 2));
            protocols.Add(new Protocol("TCP", 6));
            protocols.Add(new Protocol("UDP", 17));
            protocols.Add(new Protocol("IL", 40));
            protocols.Add(new Protocol("SDRP", 42));
            protocols.Add(new Protocol("GRE", 47));
            protocols.Add(new Protocol("ICMPv6", 58));
            protocols.Add(new Protocol("UDPLite", 136));
        }

        
        public Protocol FindProtocol(int protocolValue)
        {
            foreach(Protocol protocol in protocols)
            {
                if(protocol.Value == protocolValue)
                {
                    return protocol;
                }
            }
            return new Protocol("unknown", -1);
        }

    }
}
