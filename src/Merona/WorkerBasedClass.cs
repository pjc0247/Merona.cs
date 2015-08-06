using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Merona
{
    internal abstract class WorkerBasedClass
    {
        protected Thread thread { get; set; }
        private bool isQuitRequested { get; set; }

        public WorkerBasedClass()
        {
            this.thread = new Thread(Worker);
            this.isQuitRequested = false;
        }

        public void Start()
        {
            if (thread.IsAlive)
                throw new InvalidOperationException();

            isQuitRequested = false;
            thread.Start();
        }
        public void Kill()
        {
            if (!thread.IsAlive)
                throw new InvalidOperationException();

            isQuitRequested = true;

            thread.Interrupt();

            SpinWait.SpinUntil(() =>
            {
                return !thread.IsAlive;
            });
        }

        private void Worker()
        {
            Setup();

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
