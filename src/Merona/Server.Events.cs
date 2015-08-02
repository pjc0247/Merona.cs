using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

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
        }

		internal class AcceptEvent : Event
        {
			public TcpClient client { get; set; }

			public AcceptEvent(TcpClient client)
            {
                this.type = Type.Accept;
                this.client = client;
            }
        }
		internal class DisconnectEvent : Event
        {
            public Session session { get; set; }

            public DisconnectEvent(Session session)
            {
                this.type = Type.Disconnect;
                this.session = session;
            }
        }
		internal class RecvPacketEvent : Event
        {
			public Packet packet { get; set; }
            public Session session { get; set; }

            public RecvPacketEvent(Session session, Packet packet)
            {
                this.type = Type.RecvPacket;
                this.session = session;
                this.packet = packet;
            }
        }
    }
}
