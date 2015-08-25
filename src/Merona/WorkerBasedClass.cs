using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Merona
{
    public abstract class WorkerBasedClass
    {
        protected Thread thread { get; set; }
        private long _isRunning;
        /// <summary>
        /// Worker 쓰레드가 단순히 실행 중인지 여부를 나타낸다.
        /// </summary>
        public bool isRunning
        {
            get
            {
                return Interlocked.Read(ref _isRunning) == 1 ? true : false;
            }
        }

        /// <summary>
        /// Worker 쓰레드가 Setup처리가 완료되고 완전히 사용 가능 상태가
        /// 되었는지 여부를 나타낸다.
        /// </summary>
        private long _isInitialized;
        private bool isInitialized
        {
            get
            {
                return Interlocked.Read(ref _isInitialized) == 1 ? true : false;
            }
        }

        private bool isQuitRequested { get; set; }
        private object waitHandle;

        public WorkerBasedClass()
        {
            this.thread = new Thread(Worker);
            this.isQuitRequested = false;
            this.waitHandle = new object();
            this._isInitialized = 0;
            this._isRunning = 0;
        }

        public void Start()
        {
            if (thread.IsAlive)
                throw new InvalidOperationException();

            isQuitRequested = false;
            Interlocked.Exchange(ref _isInitialized, 0);
            Interlocked.Exchange(ref _isRunning, 1);

            thread.Start();
        }
        public void Kill()
        {
            if (!thread.IsAlive)
                throw new InvalidOperationException();

            Interlocked.Exchange(ref _isInitialized, 1);
            Interlocked.Exchange(ref _isRunning, 0);

            thread.Interrupt();

            SpinWait.SpinUntil(() =>
            {
                return !thread.IsAlive;
            });
        }

        private void Worker()
        {
            Setup();

            Interlocked.Exchange(ref _isInitialized, 1);
            lock(waitHandle)
                Monitor.Pulse(waitHandle);

            while (!isQuitRequested)
            {
                try
                {
                    WorkerRoutine();
                }
                catch (ThreadInterruptedException e)
                {
                    /* ignore */
                }
            }

            Cleanup();
        }

        /// <summary>
        /// Worker 스레드가 Setup이 완료될때까지 대기한다.
        /// </summary>
        public void Wait()
        {
            // 이미 초기화 완료된 상태
            if (Interlocked.Read(ref _isInitialized) != 0)
                return;

            // 아직 초기화 안됨.
            // 될 때 까지 대기
            lock(waitHandle)
                Monitor.Wait(waitHandle);
        }

        /// <summary>
        /// Worker에서 반복적으로 불리는 콜백
        /// </summary>
        protected abstract void WorkerRoutine();
        /// <summary>
        /// Worker 스레드의 초기화되는 시점에 불리는 콜백
        /// </summary>
        protected abstract void Setup();
        /// <summary>
        /// Worker 스레드가 종료되는 시점에 불리는 콜백
        /// </summary>
        protected abstract void Cleanup();
    }
}
