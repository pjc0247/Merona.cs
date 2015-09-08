using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merona
{
    public partial class Session
    {
        // Subscribers
        public List<IStatusSubscriber<Session>> safeCollections;

        public void InitializeSafeCollectionSupport()
        {
            safeCollections = new List<IStatusSubscriber<Session>>();
        }

        public void OnSubscribe(IStatusSubscriber<Session> safeCollection)
        {
            safeCollections.Add(safeCollection);
        }
        public void OnUnsubscribe(IStatusSubscriber<Session> safeCollection)
        {
            safeCollections.Remove(safeCollection);
        }

        public void PublishInvalidated()
        {
            var clone = new IStatusSubscriber<Session>[safeCollections.Count];

            safeCollections.CopyTo(clone);

            foreach (var safeCollection in clone)
                safeCollection.Invalidate(this);

            safeCollections.Clear();
        }
    }
}
