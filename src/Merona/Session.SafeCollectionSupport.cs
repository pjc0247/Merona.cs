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
        public List<SafeCollection<Session>> safeCollections;

        public void InitializeSafeCollectionSupport()
        {
            safeCollections = new List<SafeCollection<Session>>();
        }

        public void OnAdded(SafeCollection<Session> safeCollection)
        {
            safeCollections.Add(safeCollection);
        }
        public void OnRemoved(SafeCollection<Session> safeCollection)
        {
            safeCollections.Remove(safeCollection);
        }

        public void PublishInvalidated()
        {
            var clone = new SafeCollection<Session>[safeCollections.Count];

            safeCollections.CopyTo(clone);

            foreach (var safeCollection in clone)
                safeCollection.Invalidate(this);

            safeCollections.Clear();
        }
    }
}
