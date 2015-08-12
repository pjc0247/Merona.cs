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
        int requestId;
    }

    public partial class Cluster
    {
        private Server parent { get; set; }
        private Server server { get; set; }

        private List<Peer> peers { get; set; }

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

            this.peers = new List<Peer>();
            foreach(var peer in config.clusterPeers)
            {
                this.peers.Add(new Peer(this, peer.Item1, peer.Item2));
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

        private void ConnectToPeers()
        {
            foreach (var peer in peers)
                peer.Pairing();
        }

        internal void OnConnect(Session session)
        {
            var host = ((IPEndPoint)session.client.Client.RemoteEndPoint).Address.ToString();

            Console.WriteLine("OnConnect {0}", host);
        }
        internal void OnDisconnect(Session session)
        {
            var host = ((IPEndPoint)session.client.Client.RemoteEndPoint).Address.ToString();

            Console.WriteLine("OnDisconnect {0}", host);
        }
    }
}
