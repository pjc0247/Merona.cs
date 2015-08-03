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
        public static readonly int headerSize = Marshal.SizeOf<Packet>();
        
        [MarshalAs(UnmanagedType.I4)]
        public int size;
        [MarshalAs(UnmanagedType.I4)]
        public int packetId;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public String channel = null;


        static Packet()
        {
            InitializePreloadCaches();
        }


        internal void PreProcess(Session session)
        {
            var join = Packet.GetJoinPath(GetType());
            if (join != null)
                Server.current.channelPool.Join(join, session);

            var leave = Packet.GetJoinPath(GetType());
            if (leave != null)
                Server.current.channelPool.Leave(leave, session);   
        }
        internal void PostProcess(Session session)
        {
            var bindFields = GetBindFields(GetType());

            if(bindFields == null)
                return;

            foreach (var field in bindFields)
            {
                var bound = DataBinder.Bind(field.Item1, session);

                field.Item2.SetValue(this, bound);
            }
        }
    }
}
