using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Merona.Client
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PacketId : Attribute
    {
        public int id { get; set; }
        public PacketId(int id)
        {
            this.id = id;
        }
    }
}