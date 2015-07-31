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

            private bool isWorkerInitialized { get; set; }
            private bool isQuitReserved { get; set; }

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
                server.logger.Info("Worker::Start");

                thread.Start();

                server.logger.Debug("Worker::Start::SpinWait isWorkerInitialized");

                /* blocking wait */
                SpinWait.SpinUntil(() =>
                {
                    return isWorkerInitialized;
                });

                server.logger.Debug("Worker::Start::EndSpinWait isWorkerInitialized");
            }
            public void Kill()
            {
                isQuitReserved = true;

                server.logger.Info("Worker::Kill");
                server.logger.Debug("Worker::Kill::SpinWait thread.isAlive");

                /* blocking wait */
                SpinWait.SpinUntil(() =>
                {
                    return !thread.IsAlive;
                });

                server.logger.Debug("Worker::EndSpinWait");
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

                server.logger.Info("Worker::BeginThread tid({0})", ThreadId);

                isWorkerInitialized = true;

                while (!isQuitReserved)
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

                isWorkerInitialized = false;

                server.logger.Info("Worker::EndThraed");
            }
        }
    }
}
