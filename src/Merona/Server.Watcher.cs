using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merona
{
    public partial class Server
    {
        public class Watcher
        {
            public delegate void PacketHandler(Server sender, Packet e);
            public delegate void ConnectionHandler(Server server);
            public delegate void ExceptionHandler(Server server, Exception e);

            public event PacketHandler onSendPacket;
            public event PacketHandler onRecvPacket;
            public event ConnectionHandler onConnectClient;
            public event ConnectionHandler onDisconnectClient;
            public event ConnectionHandler onConnectCluster;
            public event ConnectionHandler onDisconnectCluster;
            public event ExceptionHandler onServerException;
            public event ExceptionHandler onUserException;

            private Server server;

            public Watcher(Server server)
            {
                this.server = server;
            }

            public void OnServerException(Exception e)
            {
                onServerException(server, e);
            }
        }

        public Watcher watcher { get; private set; }
    }
}
