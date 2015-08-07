using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Merona
{
    public sealed class Scheduler : WorkerBasedClass
    {
        abstract class Timer
        {
            public long start { get; set; }
            public long interval { get; set; }
            public long count { get; set; }
            
            public abstract void Invoke(Server server);
            public abstract bool isCancelled { get; }
        }
        class CallbackTimer : Timer
        {
            public Action callback { get; set; }
            public CancellationToken token { get; set; }

            public CallbackTimer(Action callback, CancellationToken token)
            {
                this.callback = callback;
                this.token = token;
            }

            public override void Invoke(Server server)
            {
                server.Enqueue(
                    new Server.CallFuncEvent(callback));
            }
            public override bool isCancelled
            {
                get
                {
                    return token.IsCancellationRequested;
                }
            }
        }
        class AwaitTimer : Timer
        {
            public Task task { get; set; }

            public AwaitTimer()
            {
                this.task = new Task(() => { });
            }

            public override void Invoke(Server server)
            {
                task.Start();
            }
            public override bool isCancelled
            {
                get
                {
                    return false;
                }
            }
        }

        [ThreadStatic]
        public static Scheduler current = null;

        private Server server { get; set; }
        private List<Timer> timers { get; set; }
        private ConcurrentQueue<Timer> pendingTimers { get; set; }
        private long lastTick { get; set; }

        public Scheduler(Server server)
        {
            this.server = server;
            timers = new List<Timer>();
            lastTick = Environment.TickCount;
            pendingTimers = new ConcurrentQueue<Timer>();
        }

        /// <summary>
        /// 지정된 시간 이후 콜백을 실행시킨다.
        /// 콜백은 SafeThread에서 실행이 보장된다.
        /// [Thread-Safe]
        /// </summary>
        /// <param name="callback">콜백</param>
        /// <param name="after">미룰 시간 (생략시 다음 서버 프레임에 실행됨)</param>
        /// <returns>취소 토큰</returns>
        public CancellationTokenSource Defer(Action callback,int after = 0)
        {
            return Schedule(callback, 0, after, 1);
        }

        /// <summary>
        /// 지정된 시간만큼 대기하는 Task를 생성한다.
        /// </summary>
        /// <param name="time">대기할 시간 (밀리초)</param>
        /// <returns>지정된 시간 후 완료되는 Task</returns>
        public Task Yield(int time)
        {
            var timer = new AwaitTimer();
            timer.interval = time;
            timer.count = 1;
            timer.start = Environment.TickCount;
            pendingTimers.Enqueue(timer);

            return timer.task;
        }

        /// <summary>
        /// 지정한 시간 이후에 일정 주기마다 콜백을 실행시킨다.
        /// 콜백은 SafeThread에서 실행이 보장된다.
        /// [Thread-Safe]
        /// </summary>
        /// <param name="callback">콜백</param>
        /// <param name="interval">실행 주기</param>
        /// <param name="after">미룰 시간</param>
        /// <param name="count">반복할 횟수</param>
        /// <returns>취소 토큰</returns>
        public CancellationTokenSource Schedule(Action callback, long interval, long after = 0, long count = 0)
        {
            //if (!Server.isSafeThread)
            //    throw new InvalidOperationException();

            Server.current.logger.Debug("Schedule interval({0}), after({1}), count({2})", interval, after, count);

            var cts = new CancellationTokenSource();
            var timer = new CallbackTimer(callback, cts.Token);
            timer.interval = interval;
            timer.count = count;
            timer.start = Environment.TickCount + after;
            pendingTimers.Enqueue(timer);

            return cts;
        }

        /// <summary>
        /// 취소 토큰을 이용하여 스케쥴 된 태스크를 종료시킨다.
        /// [Thread-Safe]
        /// </summary>
        /// <param name="cts"></param>
        public void Unschedule(CancellationTokenSource cts)
        {
            /* 구조상 다른 스레드에서 불러도 상관은 없음 */
            Server.current.logger.Debug("Unschedule");

            cts.Cancel();
        }

        protected override void Setup()
        {
            server.logger.Info("Scheduler::BeginWorker");
        }
        protected override void Cleanup()
        {
            server.logger.Info("Scheduler::EndWorker");            
        }
        protected override void WorkerRoutine()
        {
            var st = Environment.TickCount;

            Update();

            var elapsed = Environment.TickCount - st;

            if (elapsed <= 10)
                Thread.Sleep(10 - elapsed);
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
                    if (timer.isCancelled)
                    {
                        RemoveTimerAt(i);
                        i--;

                        continue;
                    }

                    /* invoke callback */
                    timer.Invoke(server);

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
