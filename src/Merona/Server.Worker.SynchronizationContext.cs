using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Merona
{
    public partial class Server
    {
        internal partial class Worker : WorkerBasedClass
        {
            public class WorkerSynchronizationContext : SynchronizationContext
            {
                private readonly Server server;

                public WorkerSynchronizationContext(Server server)
                {
                    this.server = server;
                }

                public override void Send(SendOrPostCallback callback, object state)
                {
                    throw new NotImplementedException("");
                }

                public override void Post(SendOrPostCallback callback, object state)
                {
                    Server.CallFuncEvent e = new Server.CallFuncEvent(() =>
                    {
                        callback.Invoke(state);
                    });

                    server.Enqueue(e);
                }
            }
        }
    }
}
