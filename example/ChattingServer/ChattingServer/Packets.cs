using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using Merona;

namespace ChattingServer
{
    class Packets
    {
        public class Join
        {
            [Merona.Join("chat.room")]
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public class C2S : Packet
            {
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
                public String nickname;

                public C2S()
                {
                    this.packetId = 1;
                }
            }
        }
        public class Leave
        {
            [Merona.Leave("chat.room")]
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public class C2S : Packet
            {
                public C2S()
                {
                    this.packetId = 2;
                }
            }
        }
        public class ChatMessage
        {
            [AutoResponse(typeof(S2C), "chat.room")]
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public class C2S : Packet
            {
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
                public String message;

                public C2S()
                {
                    this.packetId = 3;
                }
            }

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
                    this.packetId = 4;
                }
            }
        }
    }
}
