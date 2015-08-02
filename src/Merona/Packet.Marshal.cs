using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Merona
{
    public partial class Packet
    {
		public static T Deserialize<T>(byte[] buffer) where T : Packet, new()
        {
            unsafe
            {
                byte[] byteArray = new byte[Marshal.SizeOf<T>()];

                fixed (byte* ptr = &buffer[0])
                {
                    return Marshal.PtrToStructure<T>((IntPtr)ptr);
                }
            }
        }
		public byte[] Serialize()
        {
            unsafe
            {
                byte[] byteArray = new byte[Marshal.SizeOf(this)];

                fixed (byte* ptr = byteArray)
                {
                    Marshal.StructureToPtr(this, (IntPtr)ptr, true);
                }

                return byteArray;
            }
        }
    }
}
