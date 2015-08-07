using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merona
{
    public sealed partial class Server
    {
        internal class IoWorker : WorkerBasedClass
        {
            private Server server { get; set; }

            public IoWorker(Server server)
            {
                this.server = server;
            }

            protected override void Setup()
            {
            }
            protected override void Cleanup()
            {
            }
            protected override void WorkerRoutine()
            {
            }
        }
    }
}
