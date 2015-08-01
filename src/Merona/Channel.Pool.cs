using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merona
{
    public sealed partial class Channel
    {
        public class Pool
        {
            private static TreeDictionary pool;

            static Pool()
            {
                pool = new TreeDictionary();
            }

            public void Join(Channel.Path path, Session session)
            {
                if (!path.isFixed)
                    throw new ArgumentException("path isFixed(false)");

                var targets = pool.Query(path);
                if(targets.Count > 0)
                {
                    foreach (var target in targets)
                        target.Join(session);
                }
                else
                {
                    pool.Add(path).Join(session);
                }
            }
            public void Leave(Channel.Path path, Session session)
            {
                if (!path.isFixed)
                    throw new ArgumentException("path isFixed(false)");

                var targets = pool.Query(path);
                if (targets.Count > 0)
                {
                    foreach (var target in targets)
                        target.Leave(session);
                }
            }
            
            public List<Session> Query(Channel.Path path)
            {
                List<Session> results = new List<Session>();

                foreach (var channel in pool.Query(path))
                    results.AddRange(channel.Query());

                return results;
            }
        }

    }
}
