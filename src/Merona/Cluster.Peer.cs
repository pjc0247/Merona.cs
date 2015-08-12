using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Merona
{
    public partial class Cluster
    {
        public class Peer
        {
            public String host { get; private set; }
            public int port { get; private set; }
            
            public bool isActive { get; private set; }

            private Cluster parent { get; set; }
            private TcpClient client { get; set; }
            internal Session session { get; set; }

            public Peer(Cluster parent, String host, int port)
            {
                this.parent = parent;
                this.host = host;
                this.port = port;
                this.isActive = false;
                this.client = new TcpClient();
            }

            private void Connect()
            {
                parent.parent.logger.Info("Cluster::Peer::Connect - {0}:{1}", host, port);

                client.BeginConnect(
                    host, port,
                    new AsyncCallback(Connected), null);
            }
            private void Connected(IAsyncResult result)
            {
                try
                {
                    client.EndConnect(result);

                    this.isActive = true;
                }
                catch (Exception e)
                {
                    parent.parent.logger.Error("Cluster::Peer::Connected", e);
                }
            }

            public void Pairing()
            {
                Connect();
            }

            public void Reset(Merona.Session session)
            {
                this.session = (Session)session;
                this.isActive = true;
            }
        }
    }
}
