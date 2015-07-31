using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Merona
{
    public sealed class Scheduler
    {
        class Timer
        {
            public long start { get; set; }
            public long interval { get; set; }
            public long count { get; set; }
            public Action callback { get; set; }

            public CancellationToken token { get; set; }
        }

        [ThreadStatic]
        public static Scheduler current = null;

        private List<Timer> timers { get; set; }
        private List<Timer> expiredTimers { get; set; }
        private long lastTick { get; set; }

        public Scheduler()
        {
            timers = new List<Timer>();
            expiredTimers = new List<Timer>();
            lastTick = Environment.TickCount;
        }

        /// <summary>
        /// 지정된 시간 이후 콜백을 실행시킨다.
        /// 콜백은 SafeThread에서 실행이 보장된다.
        /// </summary>
        /// <param name="callback">콜백</param>
        /// <param name="after">미룰 시간 (생략시 다음 서버 프레임에 실행됨)</param>
        /// <returns>취소 토큰</returns>
        public CancellationTokenSource Defer(Action callback,int after = 0)
        {
            return Schedule(callback, 0, after, 1);
        }
        
        public CancellationTokenSource Schedule(Action callback, long interval, long after = 0, long count = 0)
        {
            if (!Server.isSafeThread)
                throw new InvalidOperationException();

            var cts = new CancellationTokenSource();
            var timer = new Timer();
            timer.interval = interval;
            timer.callback = callback;
            timer.count = count;
            timer.start = Environment.TickCount + after;
            timer.token = cts.Token;
            timers.Add(timer);

            return cts;
        }

        public void Unschedule(CancellationTokenSource cts)
        {
            /* 구조상 다른 스레드에서 불러도 상관은 없음 */
            cts.Cancel();
        }

        internal void Update()
        {
            if (!Server.isSafeThread)
                throw new InvalidOperationException();

            foreach (var timer in timers)
            {
                var elapsed = Environment.TickCount - timer.start;
                if (elapsed >= timer.interval)
                {
                    if (timer.token.IsCancellationRequested)
                    {
                        expiredTimers.Add(timer);
                        continue;
                    }

                    /* invoke callback */
                    timer.callback();

                    if (timer.count > 0 &&
                        --timer.count == 0)
                    {
                        expiredTimers.Add(timer);
                        continue;
                    }

                    timer.start = Environment.TickCount;
                }
            }

            timers = timers.Except(expiredTimers).ToList();
            expiredTimers.Clear();

            lastTick = Environment.TickCount;
        }
    }
}
