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
			public AcceptEvent(Session session)
            {
                this.type = Type.Accept;
                this.session = session;
            }
        }
		internal class DisconnectEvent : Event
        {
            public DisconnectEvent(Session session)
            {
                this.type = Type.Disconnect;
                this.session = session;
            }
        }
		internal class RecvPacketEvent : Event
        {
			public Packet packet { get; set; }

            public RecvPacketEvent(Session session, Packet packet)
            {
                this.type = Type.RecvPacket;
                this.session = session;
                this.packet = packet;
            }
        }
    }
}
