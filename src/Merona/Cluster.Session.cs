using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merona
{
    public partial class Cluster
    {
		internal class Session : Merona.Session
        {
			public Session(Server server)
            {
                this.server = server;
            }

            protected internal override void OnConnect()
            {
                server.cluster.OnConnect(this);
            }
            protected internal override void OnDisconnect()
            {
                server.cluster.OnDisconnect(this);
            }
        }
    }
}
