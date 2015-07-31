using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Reflection;

namespace Merona
{
    public partial class Packet
    {
        public String channel = null;

        static Packet()
        {
            InitializePreloadCaches();
        }

        internal void PostProcess()
        {
            var bindFields = GetBindFields(GetType());

            if(bindFields == null)
                return;

            foreach (var field in bindFields)
            {
                var bound = DataBinder.Bind(field.Item1, Session.current);

                field.Item2.SetValue(this, bound);
            }
        }
    }
}
