using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using Merona.Client;

namespace ChattingClient
{
    class Packets
    {
        public class Join
        {
            [PacketId(1)]
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
