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

                    session.FlushSend();
                }
            }

            /// <summary>
            /// IO 쓰레드에 새로운 IO 쓰기 작업이 발생했음을 알린다.
            /// 필요하다면 IO 쓰레드를 즉시 깨워 전송 작업을 시작한다.
            /// [Thread-Safe]
            /// </summary>
            /// <param name="session">IO 쓰기 작업이 발생한 세션</param>
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
