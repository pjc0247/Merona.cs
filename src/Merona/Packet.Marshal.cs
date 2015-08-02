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
        /// <summary>
        /// 바이트 배열로부터 패킷을 역직렬화한다.
        /// </summary>
        /// <typeparam name="T">패킷의 타입</typeparam>
        /// <param name="buffer">역직렬화할 바이트 배열</param>
        /// <returns>패킷</returns>
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

        /// <summary>
        /// 패킷을 바이트 배열로 직렬화한다.
        /// </summary>
        /// <returns>직렬화된 바이트 배열</returns>
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
