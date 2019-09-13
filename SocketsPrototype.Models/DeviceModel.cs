using Newtonsoft.Json;
using System;

namespace SocketsPrototype.Models
{
    public class DeviceModel
    {
        public string Address { get; set; }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public DeviceModel(string addr, Guid id, string name)
        {
            Address = addr;
            Id = id;
            Name = name;
        }
    }
}
