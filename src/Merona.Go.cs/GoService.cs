using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merona.Go
{
    using Merona;

    public class GoService : Service
    {
        private Dictionary<int, ServerObject> objects { get; set; }

        public GoService()
        {
            objects = new Dictionary<int, ServerObject>();
        }

        /// <summary>
        /// go.sync.* 채널을 구독하면서 
        /// SyncRequst 패킷을 알맞은 ServerObject에 배달한다.
        /// </summary>
        /// <param name="packet"></param>
        [Handler("go.sync.*")]
        public void OnSyncRequest(SyncProperty.Request packet)
        {
            if (objects.ContainsKey(packet.objectId) == false)
            {
                server.logger.Warn("OnSyncRequest - invalid object id {0}", packet.objectId);
                return;
            }

            var target = objects[packet.objectId];
            target.OnSyncProperty(packet);
        }
    }
}
