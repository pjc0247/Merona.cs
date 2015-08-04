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
        class ChatMessage
        {
            [Join("chat.room")]
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            class C2S
            {
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
                public String message;
            }
            [AutoResponse]
            [Join("chat.room")]
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            class S2C
            {

            }
        }
    }
}
