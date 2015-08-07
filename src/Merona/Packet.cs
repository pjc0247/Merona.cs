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
    }
}
