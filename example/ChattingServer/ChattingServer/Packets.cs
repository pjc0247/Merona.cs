using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using Merona;

namespace ChattingServer
{
    /* 이 파일은 Merona.Pgen에 의해 자동 생성됩니다 */
    /* 지금은 수동 */
    class Packets
    {
        public class Join
        {
            [PacketId(1)]
            [Merona.Join("chat.room")]
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public class C2S : Packet
            {
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
                public String nickname;

                public C2S()
                {
                    this.size = Marshal.SizeOf(GetType());
                    this.packetId = 1;
                }
            }
        }
        public class Leave
        {
            [PacketId(2)]
            [Merona.Leave("chat.room")]
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public class C2S : Packet
            {
                public C2S()
                {
                    this.size = Marshal.SizeOf(GetType());
                    this.packetId = 2;
                }
            }
        }
        public class ChatMessage
        {
            [PacketId(3)]
            [AutoResponse(typeof(ChatMessage.S2C), "chat.room")]
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public class C2S : Packet
            {
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
                public String message;

                public C2S()
                {
                    this.size = Marshal.SizeOf(GetType());
                    this.packetId = 3;
                }
            }

            [PacketId(4)]
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public class S2C : Packet
            {
                [Bind("#{nickname}")]
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
                public String nickname;

                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
                public String message;

                public S2C()
                {
                    this.size = Marshal.SizeOf(GetType());
                    this.packetId = 4;
                }
            }
        }
    }
}
