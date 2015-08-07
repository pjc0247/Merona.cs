using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merona
{
    internal sealed class LinearBuffer
    {
        private byte[] buffer { get; set; }
        private int rearPtr { get; set; }
        private int capacity { get; set; }

        public byte[] writableBuffer
        {
            get
            {
                return buffer;
            }
        }
        public int writableOffset
        {
            get
            {
                return rearPtr;
            }
        }

        public LinearBuffer(int capacity)
        {
            this.buffer = new byte[capacity];
            this.capacity = capacity;
        }

        public void Commit(int size)
        {
            rearPtr += size;

            if (rearPtr >= capacity)
                throw new InternalBufferOverflowException();
        }
        public void Peek(byte[] data, int size)
        {

        }
        public int PeekInt(int offset = 0)
        {
            return BitConverter.ToInt32(buffer, offset);
        }
        public void Consume(int size)
        {
        }
    }
}
