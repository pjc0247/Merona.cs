using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Merona
{
    public sealed partial class Server
    {
        private class Worker
        {
            private Server server { get; set; }
            private Thread thread { get; set; }
            private Scheduler scheduler { get; set; }

            public int ThreadId {
                get{
                    return thread.ManagedThreadId;
                }
            }

            public Worker(Server server)
            {
                this.server = server;
                this.scheduler = new Scheduler();
                this.thread = new Thread(Loop);
            }

            public void Start()
            {
                server.logger.Info("Start Worker");

                thread.Start();
            }
            private void Loop(object arg)
            {
                /* initialize thread-locals */
                Server.current = server;
                Scheduler.current = scheduler;
                Session.current = new Session(); //for test

                var cts = new CancellationTokenSource();
                var t = new Timer(delegate(object _arg)
                {
                    cts.Cancel();
                    Interlocked.Exchange(ref cts, new CancellationTokenSource());
                }, null, 0, 30); /* TODO : config */

                server.logger.Info("Worker Initialized tid : {0}", ThreadId);

                while (true)
                {
                    Tuple<Session,Packet> packet = null;

                    /* packet */
                    try
                    {
                        packet = server.pendingPackets.Take(cts.Token);
                        
                        foreach (var service in server.services)
                        {
                            Session.current = packet.Item1;
                            var routed = service.Route(packet.Item2);
                        }
                    }
                    catch (OperationCanceledException e)
                    {
                        /* Take() operation canceled */
                    }

                    /* scheduler */
                    scheduler.Update();

                    /* accept */
                    while (server.pendingClients.Count > 0)
                    {
                        Session client = null;
                        if (server.pendingClients.TryTake(out client))
                        {
                            client.OnConnect();
                        }
                    }
                }
            }
        }
    }
}
