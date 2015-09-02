using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;

namespace Merona
{
    public partial class Session
    {
        internal class Pool : IDisposable
        {
            public int size { get; private set; }

            private ConcurrentStack<Session> pool { get; set; }

            public Pool(int size)
            {
                if (size <= 0)
                    throw new ArgumentException();

                this.size = size;
                this.pool = new ConcurrentStack<Session>();

                for (int i = 0; i < size; i++)
                {
                    var session = 
                        (Session)Activator.CreateInstance(Server.current.config.sessionType);
                    session.server = Server.current;
                    this.pool.Push((Session)session);
                }
            }

            public void Dispose()
            {
                Server.current.logger.Info("Session.Pool::Dispose");

                foreach(var session in pool){
                    session.Disconnect();
                }

                pool.Clear();
            }

            public Session Acquire()
            {
                Session session = null;

                if (pool.TryPop(out session))
                {
                    return session;
                }
                else
                {
                    /* underflow */
                    return null;
                }
            }
            public void Return(Session session)
            {
                if (pool.Count >= size)
                    throw new InvalidOperationException();

                pool.Push(session);
            }
        }
    }
}
