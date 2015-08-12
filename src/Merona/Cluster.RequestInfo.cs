using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merona
{
    public partial class Cluster
    {
        internal class RequestInfo
        {
            public int requestId { get; set; }

            public int sentAt { get; set; }

            public Task<ClusterPacket> task { get; set; }
            public ClusterPacket response { get; set; }
        }
    }
}
