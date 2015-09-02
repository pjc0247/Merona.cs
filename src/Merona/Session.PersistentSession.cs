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
                .Where(type => type.IsSubclassOf(typeof(Session)));

            foreach (var child in children)
            {
                var persistentSessions = child.GetProperties()
                    .Where(prop => prop.PropertyType.IsSubclassOf(typeof(PersistentSession)));

                persistentSessionCaches[child] = persistentSessions.ToList();
            }
        }

        internal Task CommmitAllPersistentSessions()
        {
            var persistentSessions = persistentSessionCaches[GetType()];
            var tasks = new List<Task>();

            foreach (var persistentSessionProp in persistentSessions) {
                var persistentSession = (PersistentSession)persistentSessionProp.GetValue(this);

                tasks.Add(persistentSession.Commit());
            }

            return Task.WhenAll(tasks);
        }
    }
}
