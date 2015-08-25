using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Merona
{
    public class Module : WorkerBasedClass
    {
        /// <summary>
        /// 현재 Worker를 가지고 있는 모듈의 인스턴스
        /// 이 값은 Worekr 스레드에서만 유효하다
        /// </summary>
        [ThreadStatic]
        public static Module current = null;

        /// <summary>
        /// 현재 스레드가 Module.current에 대해 안전한지 조사한다.
        /// </summary>
        public static bool isSafeThread
        {
            get
            {
                if (current == null)
                    return false;
                return current.thread.ManagedThreadId == Thread.CurrentThread.ManagedThreadId;
            }
        }

        protected override void Setup()
        {

        }
        protected override void Cleanup()
        {

        }
        protected override void WorkerRoutine()
        {

        }

        /// <summary>
        /// 서버에 서비스를 추가한다.
        /// 이 동작은 서버가 실행중이 아닐 때만 수행할 수 있다.
        /// [Non-Thread-Safe]
        /// </summary>
        /// <param name="service">추가할 서비스</param>
        public void AttachService<T>() where T : Service, new()
        {
            /* 실행 중에는 서비스가 추가될 수 없음 */
            if (isRunning)
                throw new InvalidOperationException("");

            logger.Info("Attach {0}", typeof(T).Name);

            var service = new T();
            service.server = this;
            services.Add(service);
            service.server = this;
        }

        /// <summary>
        /// 미처리 이벤트 목록에 이벤트를 담는다.
        /// 이 이벤트들은 주기적으로 Worker에 의해서 Consume된다.
        /// [Thread-Safe]
        /// </summary>
        /// <param name="ev">이벤트</param>
        internal void Enqueue(Event ev)
        {
            pendingEvents.Add(ev);
        }
    }
}
