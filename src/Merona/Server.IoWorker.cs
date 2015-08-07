using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Merona
{
    public sealed partial class Server
    {
        internal class IoWorker : WorkerBasedClass
        {
            private object obj;
            private Server server { get; set; }
            private ConcurrentDictionary<Session, byte> pendingSessions;

            public IoWorker(Server server)
            {
                this.server = server;
                this.obj = new object();
                this.pendingSessions = new ConcurrentDictionary<Session, byte>();
            }

            protected override void Setup()
            {
            }
            protected override void Cleanup()
            {
            }
            protected override void WorkerRoutine()
            {
                lock (obj)
                {
                    Monitor.Wait(obj);
                }

                foreach (var session in pendingSessions)
                {
                    byte trash;
                    // TryRemove가 실패한 경우는, 이미 누가 이 세션을 처리중에 있는것이니 넘김
                    if (!pendingSessions.TryRemove(session.Key, out trash))
                        continue;

                    var skip = Interlocked.Exchange(ref session.Key.skip, 0);
                    session.Key.sendRingBuffer.Skip((int)skip);

                    foreach (var packet in session.Key.pendingPackets)
                    {
                        var buffer = packet.Serialize();

                        session.Key.sendRingBuffer.Put(buffer);
                    }

                    var count = session.Key.sendRingBuffer.Size;
                    var bufferToSend = new byte[count];

                    session.Key.sendRingBuffer.Peek(bufferToSend, 0, count);
                    session.Key.client.Client.BeginSend(
                        bufferToSend, 0, count, SocketFlags.None,
                        new AsyncCallback(session.Key.Sent), count);
                }
            }

            public void Pulse(Session session)
            {
                if (!pendingSessions.TryAdd(session, 0))
                    return;

                lock (obj)
                {
                    Monitor.Pulse(obj);
                }   
            }
        }
    }
}
