using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Merona
{
    class Endians
    {
        public enum ByteOrder
        {
            LittleEndian,
            BigEndian
        }

        /// <summary>
        /// 현재 시스템의 바이트 오더 타입을 가져온다
        /// </summary>
        public static ByteOrder systemByteOrder
        {
            get
            {
                return BitConverter.IsLittleEndian ? ByteOrder.LittleEndian : ByteOrder.BigEndian;
            }
        }
        /// <summary>
        /// 타겟 바이트 오더 타입을 가져오거나 설정한다.
        /// </summary>
        public static ByteOrder targetByteOrder { get; set; }

        static Endians()
        {
            targetByteOrder = systemByteOrder;
        }

        /// <summary>
        /// 바이트 배열을 타겟 바이트 오더에 맞게 재정렬한다.
        /// </summary>
        public static void ReorderEndians(Type type, byte[] data)
        {
            foreach (FieldInfo f in type.GetFields())
            {
               int offset = Marshal.OffsetOf(type, f.Name).ToInt32();

               if(systemByteOrder != targetByteOrder)
               {
                   Array.Reverse(data, offset, Marshal.SizeOf(f.FieldType));
               }
            }
        }
    }
}
