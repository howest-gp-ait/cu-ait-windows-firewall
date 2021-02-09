using System;
using System.Collections.Generic;
using System.Text;

namespace Ait.WindowsFirewall.Core.Entities
{
    public class Protocol
    {
        public string Name{ get; private set; }
        public int Value { get; private set; }
        public Protocol(string name, int value)
        {
            Name = name;
            Value = value;
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
