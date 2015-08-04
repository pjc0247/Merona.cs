using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Reflection;

namespace Merona.Client
{
    public partial class Packet
    {
        private static Dictionary<int, Type> types;

        static Packet()
        {
            types = new Dictionary<int, Type>();

            var packets = Assembly.GetEntryAssembly().GetTypes()
                .Where(type => type.IsSubclassOf(typeof(Packet)));

            foreach (var packet in packets)
            {
                var id = (PacketId)packet.GetCustomAttribute(typeof(PacketId));
                if (id != null)
                    types[id.id] = packet;
            }
        }

        public static Type GetTypeById(int id)
        {
            return types[id];
        }
    }
}