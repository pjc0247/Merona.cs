using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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

        private Server server { get; set; }
        private List<Timer> timers { get; set; }
        private ConcurrentQueue<Timer> pendingTimers { get; set; }
        private long lastTick { get; set; }

        private Thread thread { get; set; }

        public Scheduler(Server server)
        {
            this.server = server;
            timers = new List<Timer>();
            lastTick = Environment.TickCount;
            pendingTimers = new ConcurrentQueue<Timer>();

            thread = new Thread(Worker);
            thread.Start();
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
            //if (!Server.isSafeThread)
            //    throw new InvalidOperationException();

            Server.current.logger.Debug("Schedule interval({0}), after({1}), count({2})", interval, after, count);

            var cts = new CancellationTokenSource();
            var timer = new Timer();
            timer.interval = interval;
            timer.callback = callback;
            timer.count = count;
            timer.start = Environment.TickCount + after;
            timer.token = cts.Token;
            pendingTimers.Enqueue(timer);

            return cts;
        }

        public void Unschedule(CancellationTokenSource cts)
        {
            /* 구조상 다른 스레드에서 불러도 상관은 없음 */
            Server.current.logger.Debug("Unschedule");

            cts.Cancel();
        }

        private void Worker()
        {
            server.logger.Info("Scheduler::BeginWorker");

            /* TODO : 종료 조건 */
            while (true)
            {
                var st = Environment.TickCount;

                Update();

                var elapsed = Environment.TickCount - st;

                if (elapsed <= 10)
                    Thread.Sleep(10 - elapsed);
            }

            server.logger.Info("Scheduler::EndWorker");
        }

        private void RemoveTimerAt(int i)
        {
            timers[i] = timers[timers.Count - 1];
            timers.RemoveAt(timers.Count - 1);
        }
        private void Update()
        {
            while (pendingTimers.Count > 0)
            {
                Timer timer;
                pendingTimers.TryDequeue(out timer);

                timers.Add(timer);
            }
            
            for(var i=0;i<timers.Count;i++)
            {
                var timer = timers[i];

                var elapsed = Environment.TickCount - timer.start;
                if (elapsed >= timer.interval)
                {
                    if (timer.token.IsCancellationRequested)
                    {
                        RemoveTimerAt(i);
                        i--;

                        continue;
                    }

                    /* invoke callback */
                    server.Enqueue(
                        new Server.CallFuncEvent(timer.callback));

                    if (timer.count > 0 &&
                        --timer.count == 0)
                    {
                        /* swap remove */
                        RemoveTimerAt(i);
                        i--;

                        continue;
                    }

                    timer.start = Environment.TickCount;
                }
            }
            
            lastTick = Environment.TickCount;
        }
    }
}
