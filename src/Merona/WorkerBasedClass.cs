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
        private bool isQuitRequested { get; set; }

        public WorkerBasedClass()
        {
            this.thread = new Thread(Worker);
            this.isQuitRequested = false;
        }

        public void Start()
        {
            isQuitRequested = false;
            thread.Start();
        }
        public void Kill()
        {
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

        protected abstract void WorkerRoutine();
        protected abstract void Setup();
        protected abstract void Cleanup();
    }
}
