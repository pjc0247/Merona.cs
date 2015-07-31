using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Merona
{
    /// <summary>
    /// 스레드에 안전한 중복되지 않는 id 발급기
    /// </summary>
    public sealed class IdDispenser
    {
        private Int32 current;

        public Int32 next
        {
            get
            {
                return Interlocked.Add(ref current, 1);
            }
        }

        public IdDispenser()
        {
            this.current = 0;
        }
    }

    /// <summary>
    /// 스레드에 안전한 중복되지 않는 id 발급기 (Int64)
    /// </summary>
    public sealed class IdDispenser64
    {
        private Int64 current;

        public Int64 next
        {
            get
            {
                return Interlocked.Add(ref current, 1);
            }
        }

        public IdDispenser64()
        {
            this.current = 0;
        }
    }
}
