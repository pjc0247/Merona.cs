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

            public void Join(Channel channel)
            {

            }
            public void Leave(Channel channel)
            {

            }

            // test-in-1 : world.map
            // test-in-2 : world.map.* 
            public List<Session> Query(Channel channel)
            {
                return null;
                //return pool.Query(channel);
            }
        }

    }
}
