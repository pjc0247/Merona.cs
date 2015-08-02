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
            return (T)Deserialize(buffer, typeof(T));
        }

        /// <summary>
        /// 바이트 배열로부터 패킷을 역직렬화한다.
        /// </summary>
        /// <param name="buffer">역직렬화할 바이트 배열</param>
        /// <param name="type">패킷의 타입</param>
        /// <returns>패킷</returns>
        public static Packet Deserialize(byte[] buffer, Type type)
        {
#if MERONA_PACKET_MARSHAL_UNSAFE
            unsafe
            {
                byte[] byteArray = new byte[Marshal.SizeOf(type)];

                fixed (byte* ptr = &buffer[0])
                {
                    return (Packet)Marshal.PtrToStructure((IntPtr)ptr, type);
                }
            }
#else
            throw new NotImplementedException();
#endif
        }

        /// <summary>
        /// 패킷을 바이트 배열로 직렬화한다.
        /// </summary>
        /// <returns>직렬화된 바이트 배열</returns>
        public byte[] Serialize()
        {
#if MERONA_PACKET_MARSHAL_UNSAFE
            unsafe
            {
                byte[] byteArray = new byte[Marshal.SizeOf(this)];

                fixed (byte* ptr = byteArray)
                {
                    Marshal.StructureToPtr(this, (IntPtr)ptr, true);
                }

                return byteArray;
            }
#else
            throw new NotImplementedException();
#endif
        }
    }
}
