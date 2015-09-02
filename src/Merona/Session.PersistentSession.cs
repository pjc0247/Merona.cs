using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Merona
{
    public partial class Session
    {
        private static Dictionary<Type, List<PropertyInfo>> persistentSessionCaches { get; set; }

        static Session()
        {
            persistentSessionCaches = new Dictionary<Type, List<PropertyInfo>>();

            var children = Assembly.GetEntryAssembly().GetTypes()
                .Where(type => 
                    type.IsEquivalentTo(typeof(Session)) || 
                    type.IsSubclassOf(typeof(Session)));

            foreach (var child in children)
            {
                var persistentSessions = child.GetProperties()
                    .Where(prop => prop.PropertyType.IsSubclassOf(typeof(PersistentSession)));

                persistentSessionCaches[child] = persistentSessions.ToList();
            }
        }

        internal enum PersistentSessionCommitTiming
        {
            AfterRequest,
            AfterSessionClosed
        }

        internal Task CommmitPersistentSessionsAsync(
            PersistentSessionCommitTiming timing)
        {
            var persistentSessions = persistentSessionCaches[GetType()];
            var tasks = new List<Task>();

            foreach (var persistentSessionProp in persistentSessions) {
                var persistentSession = (PersistentSession)persistentSessionProp.GetValue(this);

                if (persistentSession.autoCommitType == PersistentSession.AutoCommitType.None)
                    continue;
                if (persistentSession.autoCommitType == PersistentSession.AutoCommitType.AfterRequest &&
                    timing != PersistentSessionCommitTiming.AfterRequest)
                    continue;
                if (persistentSession.autoCommitType == PersistentSession.AutoCommitType.AfterSessionClosed &&
                    timing != PersistentSessionCommitTiming.AfterSessionClosed)
                    continue;

                tasks.Add(persistentSession.CommitAsync());
            }

            return Task.WhenAll(tasks);
        }
    }
}
