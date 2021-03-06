﻿using System;
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
            public delegate void PipelineHandler(Server sender, Session session);
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
            public event PipelineHandler onBeginPipeline;
            public event PipelineHandler onEndPipeline;

            private Server server;

            public Watcher(Server server)
            {
                this.server = server;
            }

            public void OnServerException(Exception e)
            {
                onServerException?.Invoke(server, e);
            }
            public void OnUserException(Exception e)
            {
                onUserException?.Invoke(server, e);
            }

            public void OnBeginPipeline(Session session)
            {
                onBeginPipeline?.Invoke(server, session);
            }
            public void OnEndPipeline(Session session)
            {
                onEndPipeline?.Invoke(server, session);
            }
        }

        public Watcher watcher { get; private set; }
    }
}
