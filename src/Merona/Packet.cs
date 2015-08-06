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


        /// <summary>
        /// 현재 패킷과, 지정된 세션에 대해서 인바인딩을 수행한다.
        /// (패킷 필드의 값을 세션의 프로퍼티에 바인딩)
        /// </summary>
        /// <param name="session">세션</param>
        private void InBind(Session session)
        {
            var bindFields = GetBindFields(GetType());
            if (bindFields == null)
                return;

            foreach (var field in bindFields)
            {
                var value = field.Item2.GetValue(this);
                DataBinder.InBind(field.Item1, value, session);
            }
        }
        /// <summary>
        /// 현재 패킷과, 지정된 세션에 대해서 아웃바인딩을 수행한다.
        /// (세션 프로퍼티의 값을 패킷의 필드에 바인딩한다)
        /// </summary>
        /// <param name="session">세션</param>
        private void OutBind(Session session)
        {
            var bindFields = GetBindFields(GetType());
            if (bindFields == null)
                return;

            foreach (var field in bindFields)
            {
                DataBinder.OutBind(field.Item1, session, field.Item2, this);
            }
        }
        /// <summary>
        /// 패킷이 각 서비스들에게 라우팅 되기 전 처리해야 할 작업들을 수행한다.
        /// 이 작업은 C2S 패킷에만 수행되어야 한다.
        /// </summary>
        /// <param name="session">세션</param>
        internal void PreProcess(Session session)
        {
            var join = Packet.GetJoinPath(GetType());
            if (join != null)
                Server.current.channelPool.Join(join, session);

            var leave = Packet.GetJoinPath(GetType());
            if (leave != null)
                Server.current.channelPool.Leave(leave, session);

            InBind(session);
        }
        /// <summary>
        /// 패킷이 클라이언트에게 Send 되기 전 처리해야 할 작업들을 수행한다.
        /// 이 작업은 S2C 패킷에만 수행되어야 한다.
        /// </summary>
        /// <param name="session">세션</param>
        internal void PostProcess(Session session)
        {
            OutBind(session);
        }
    }
}
