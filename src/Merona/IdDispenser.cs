﻿using System;
using System.Threading;

namespace Merona
{
    /// <summary>
    /// 스레드에 안전한 중복되지 않는 id 발급기
    /// </summary>
    public sealed class IdDispenser
    {
        private Int32 current;

        /// <summary>
        /// 중복되지 않는 아이디를 발급받는다.
        /// [Thread-Safe]
        /// </summary>
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

        /// <summary>
        /// 중복되지 않는 아이디를 발급받는다.
        /// [Thread-Safe]
        /// </summary>
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
