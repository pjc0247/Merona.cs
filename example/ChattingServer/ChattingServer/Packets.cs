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
            }
        }
        public class Leave
        {
            [Merona.Leave("chat.room")]
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public class C2S : Packet
            {
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
            }

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public class S2C : Packet
            {
                [Bind("#{nickname}")]
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
                public String nickname;

                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
                public String message;
            }
        }
    }
}
