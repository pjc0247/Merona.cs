using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Runtime.InteropServices;

namespace Merona.Client
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public partial class Packet
    {
        public int id;
        public int packetId;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public String channel;
    }
}
