using System;
using System.Collections.Generic;
using System.Text;
using NetFwTypeLib;
using System.Linq;

namespace Ait.WindowsFirewall.Core.Services
{
    public class FirewallService
    {
        private List<INetFwRule> outboundRules;
        private List<INetFwRule> inboundRules;

        public List<INetFwRule> OutboundRules
        {
            get { return outboundRules; }
        }
        public List<INetFwRule> InboundRules
        {
            get { return inboundRules; }
        }
        public FirewallService()
        {
            outboundRules = new List<INetFwRule>();
            inboundRules = new List<INetFwRule>();
            INetFwPolicy2 netFwPolicy2 = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
            foreach(INetFwRule netFwRule in netFwPolicy2.Rules)
            {
                if (netFwRule.Direction == NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT)
                    outboundRules.Add(netFwRule);
                if (netFwRule.Direction == NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN)
                    inboundRules.Add(netFwRule);
            }
            outboundRules = outboundRules.OrderBy(r => r.Name).ToList();
            inboundRules = inboundRules.OrderBy(r => r.Name).ToList();
        }
        public bool CreateOutboundRule(string name, string description, int profileValue, bool enabled, NET_FW_ACTION_ typeOfRule, string application, string localAdresses, string remoteAdresses, string localPorts, string remotePorts, int protocolValue)
        {
            return CreateRule(NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT, name, description, profileValue, enabled, typeOfRule, application, localAdresses, remoteAdresses, localPorts, remotePorts, protocolValue);
        }
        public bool CreateInboundRule(string name, string description, int profileValue, bool enabled, NET_FW_ACTION_ typeOfRule, string application, string localAdresses, string remoteAdresses, string localPorts, string remotePorts, int protocolValue)
        {
            return CreateRule(NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN, name, description, profileValue, enabled, typeOfRule, application, localAdresses, remoteAdresses, localPorts, remotePorts, protocolValue);
        }
        private bool CreateRule(NET_FW_RULE_DIRECTION_ richting, string name, string description, int profileValue, bool enabled, NET_FW_ACTION_ typeOfRule, string application, string localAdresses, string remoteAdresses, string localPorts, string remotePorts, int protocolValue)
        {
            INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
            INetFwRule firewallRule = firewallPolicy.Rules.OfType<INetFwRule>().Where(x => x.Name == name).FirstOrDefault();
            if (firewallRule != null)
            {
                firewallPolicy.Rules.Remove(firewallRule.Name);
            }
            try
            {
                firewallRule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
                firewallRule.Name = name;
                firewallPolicy.Rules.Add(firewallRule);
                firewallRule.Description = description;
                firewallRule.Grouping = "testomgeving .Net";
                firewallRule.Profiles = profileValue;
                firewallRule.Protocol = protocolValue;  // moet gezet worden vooraleer de poorten gezet worden
                firewallRule.Direction = richting;
                firewallRule.Action = typeOfRule;
                firewallRule.ApplicationName = application;
                firewallRule.LocalAddresses = localAdresses;
                if (localPorts != "")
                    firewallRule.LocalPorts = localPorts;
                firewallRule.RemoteAddresses = remoteAdresses;
                if (remotePorts != "")
                    firewallRule.RemotePorts = remotePorts;
                firewallRule.Enabled = enabled;
            }
            catch 
            {
                return false;
            }
            return true;
        }
    }
}
