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
        private class Worker : WorkerBasedClass
        {
            private Server server { get; set; }

            private bool isWorkerInitialized { get; set; }
            private CancellationTokenSource cts;

            public int ThreadId {
                get{
                    return thread.ManagedThreadId;
                }
            }

            public Worker(Server server)
            {
                this.server = server;
            }

            protected override void Setup()
            {
                /* initialize thread-locals */
                Server.current = server;
                Scheduler.current = server.scheduler;
                Session.current = new Session(); //for test

                cts = new CancellationTokenSource();
                var t = new Timer(delegate(object _arg)
                {
                    cts.Cancel();
                    Interlocked.Exchange(ref cts, new CancellationTokenSource());
                }, null, 0, 30); /* TODO : config */

                server.logger.Info("Worker::Setup tid({0})", ThreadId);

                isWorkerInitialized = true;
            }
            protected override void Cleanup()
            {
                isWorkerInitialized = false;

                server.logger.Info("Worker::Cleanup");
            }
            protected override void WorkerRoutine()
            {
                Event ev = null;

                try
                {
                    ev = server.pendingEvents.Take(cts.Token);

                    if (ev.type == Event.Type.Accept)
                        OnAccept((AcceptEvent)ev);
                    else if (ev.type == Event.Type.Disconnect)
                        OnDisconnect((DisconnectEvent)ev);
                    else if (ev.type == Event.Type.RecvPacket)
                        OnRecvPacket((RecvPacketEvent)ev);
                    else if (ev.type == Event.Type.CallFunc)
                        OnCallFunc((CallFuncEvent)ev);
                }
                catch (OperationCanceledException e)
                {
                    /* Take() operation canceled */
                }
            }

            private void OnAccept(AcceptEvent e)
            {
                Session session;

                session = server.sessionPool.Acquire();
                if(session != null)
                {
                    session.Reset(e.client);
                    session.OnConnect();
                }
                else
                    server.logger.Warn("Worker::OnAccept - sessionPool underflow");
            }
            private void OnDisconnect(DisconnectEvent e)
            {
                e.session.OnDisconnect();

                server.sessionPool.Return(e.session);
            }
            private void OnRecvPacket(RecvPacketEvent e)
            {
                foreach (var service in server.services)
                {
                    Session.current = e.session;
                    var routed = service.Route(e.packet);
                }
            }
            private void OnCallFunc(CallFuncEvent e)
            {
                e.callback();
            }
        }
    }
}
