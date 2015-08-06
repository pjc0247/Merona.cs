using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Merona
{
    public sealed partial class Server
    {
        /// <summary>
        /// 모든 이벤트의 베이스 클래스
        /// </summary>
		internal class Event
        {
            internal enum Type
            {
                Accept,
                Disconnect,
                RecvPacket,
				CallFunc
            }

            public Type type { get; set; }
        }

        /// <summary>
        /// IO 쓰레드에서 수행된 Accept를 알려주기 위한 이벤트
        /// </summary>
		internal class AcceptEvent : Event
        {
			public TcpClient client { get; set; }

			public AcceptEvent(TcpClient client)
            {
                this.type = Type.Accept;
                this.client = client;
            }
        }
        /// <summary>
        /// IO 쓰레드 혹은 Worker 쓰레드에서 세션이 끊어짐을 알려주기 위한 이벤트
        /// </summary>
		internal class DisconnectEvent : Event
        {
            public Session session { get; set; }

            public DisconnectEvent(Session session)
            {
                this.type = Type.Disconnect;
                this.session = session;
            }
        }
        /// <summary>
        /// IO 쓰레드에서 패킷이 수신됨을 알려주기 위한 이벤트
        /// </summary>
		internal class RecvPacketEvent : Event
        {
			public Packet packet { get; set; }
            public Session session { get; set; }

            public RecvPacketEvent(Session session, Packet packet)
            {
                this.type = Type.RecvPacket;
                this.session = session;
                this.packet = packet;
            }
        }
        /// <summary>ㄴ
        /// 안전하지 않은 쓰레드에서 Worker 쓰레드에 함수 호출을 요청하기 위한 이벤트
        /// </summary>
        /// <remarks>
        /// 이 이벤트를 사용하여 안전하지 않은 쓰레드에서 안전한 쓰레드로
        /// 처리 흐름을 변경할 수 있다.
        /// </remarks>
		internal class CallFuncEvent : Event
        {
			public Action callback { get; set; }

			public CallFuncEvent(Action callback)
            {
                this.type = Type.CallFunc;
                this.callback = callback;
            }
        }
    }
}
