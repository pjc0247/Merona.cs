using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Merona
{
    public class ClusterPacket : Packet
    {
        public int requestId;
    }

    public partial class Cluster
    {
        private Server parent { get; set; }
        private Server server { get; set; }

        private Dictionary<int, Peer> peers { get; set; }
        private Dictionary<int, RequestInfo> pendings { get; set; }
        private IdDispenser dispenser { get; set; }

        public Cluster(Server parent)
        {
            var config = (Config)parent.config.Clone();

            config.enableCluster = false;
            config.enableDB = false;
            config.port = config.clusterPort;
            config.sessionType = typeof(Cluster.Session);

            this.parent = parent;
            this.server = new Server(config);
            this.server.cluster = this;
            this.dispenser = new IdDispenser();

            this.peers = new Dictionary<int, Peer>();
            this.pendings = new Dictionary<int, RequestInfo>();

            int idx = 0;
            foreach(var peer in config.clusterPeers)
            {
                this.peers[idx++] = new Peer(this, peer.Item1, peer.Item2);
            }
        }

        public void Start()
        {
            parent.logger.Info("Cluster::Start");

            this.server.Start();
            ConnectToPeers();
        }
        public void Kill()
        {
            parent.logger.Info("Cluster::Kill");

            this.server.Kill();
        }

        public Task<ClusterPacket> Send(int dst, ClusterPacket packet)
        {
            var peer = peers[dst];

            if (!peer.isActive)
                throw new InvalidOperationException("peer is not active");

            var id = dispenser.next;
            var requestInfo = new RequestInfo();
            requestInfo.requestId = id;
            requestInfo.sentAt = Environment.TickCount;
            pendings[id] = requestInfo;
            packet.requestId = id;
            peer.session.Send(packet);

            return new Task<ClusterPacket>(()=>
            {
                return requestInfo.response;
            });
        }

        public void Broadcast(Channel.Path dst, ClusterPacket packet)
        {

        }
        public void Broadcast(String dst, ClusterPacket packet)
        {
            Broadcast(new Channel.Path(dst), packet);
        }

        private void ConnectToPeers()
        {
            foreach (var peer in peers)
                peer.Value.Pairing();
        }

        internal void OnConnect(Session session)
        {
            var endPoint = ((IPEndPoint)session.client.Client.RemoteEndPoint);

            foreach(var peer in peers)
            {
                if(peer.Value.host == endPoint.Address.ToString() &&
                   peer.Value.port == endPoint.Port)
                {
                    peer.Value.Reset(session);
                    break;
                }
            }

            Console.WriteLine("OnConnect {0}", endPoint.Address.ToString());
        }
        internal void OnDisconnect(Session session)
        {
            var host = ((IPEndPoint)session.client.Client.RemoteEndPoint).Address.ToString();

            Console.WriteLine("OnDisconnect {0}", host);
        }
    }
}
