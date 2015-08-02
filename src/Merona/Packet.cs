using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Merona
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public partial class Packet
    {
        [MarshalAs(UnmanagedType.I4)]
        public int id;
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
