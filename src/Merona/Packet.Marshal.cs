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
            return new T();
        }
		public byte[] Serialize()
        {
            Console.WriteLine("serialize");
            Console.WriteLine(Marshal.SizeOf(this));

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
