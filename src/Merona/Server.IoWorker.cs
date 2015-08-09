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

                foreach (var _session in pendingSessions)
                {
                    var session = _session.Key;
                    byte trash;
                    // TryRemove가 실패한 경우는, 이미 누가 이 세션을 처리중에 있는것이니 넘김
                    if (!pendingSessions.TryRemove(session, out trash))
                        continue;

                    var skip = Interlocked.Exchange(ref session.skip, 0);
                    session.sendRingBuffer.Skip((int)skip);

                    while (true)
                    {
                        var serialized =
                            session.marshaler.Serialize(session.pendingPackets);

                        if (serialized == null)
                            break;

                        session.sendRingBuffer.Put(serialized);
                    }
                    
                    var count = session.sendRingBuffer.Size;
                    var bufferToSend = new byte[count];

                    session.sendRingBuffer.Peek(bufferToSend, 0, count);
                    session.client.Client.BeginSend(
                        bufferToSend, 0, count, SocketFlags.None,
                        new AsyncCallback(session.Sent), count);
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
