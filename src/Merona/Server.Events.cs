using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merona
{
    public sealed partial class Server
    {
		internal class Event
        {
            internal enum Type
            {
                Accept,
                Disconnect,
                RecvPacket
            }

            public Type type { get; set; }
			public Session session { get; set; }
        }

		internal class AcceptEvent : Event
        {
			public AcceptEvent(Type type, Session session)
            {
                this.type = type;
                this.session = session;
            }
        }
		internal class DisconnectEvent : Event
        {
            public DisconnectEvent(Type type, Session session)
            {
                this.type = type;
                this.session = session;
            }
        }
		internal class RecvPacketEvent : Event
        {
			public Packet packet { get; set; }

            public RecvPacketEvent(Type type, Session session, Packet packet)
            {
                this.type = type;
                this.session = session;
                this.packet = packet;
            }
        }
    }
}
