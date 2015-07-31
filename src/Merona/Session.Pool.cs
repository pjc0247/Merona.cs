using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merona
{
    public partial class Session
    {
        public class Pool
        {
            public int size { get; private set; }

            private ConcurrentStack<Session> pool { get; set; }

            public Pool(int size)
            {
                this.size = size;
                this.pool = new ConcurrentStack<Session>();

                for (int i = 0; i < size; i++)
                {
                    this.pool.Push(new Session());
                }
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
                pool.Push(session);
            }
        }
    }
}
