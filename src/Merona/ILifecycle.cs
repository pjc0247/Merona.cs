using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merona
{
    /// <summary>
    /// 초기화 상태, 시작 상태, 종료 상태를 가지는 오브젝트의 베이스
    /// </summary>
    public abstract class ILifecycle
    {
        /// <summary>
        /// 오브젝트를 시작하는 루틴을 상속받아 구현한다.
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// 오브젝트를 종료하는 루틴을 상속받아 구현한다.
        /// </summary>
        public abstract void Kill();

        /// <summary>
        /// 오브젝트의 시작이 완료될 때 까지 대기하는 루틴을 상속받아 구현한다.
        /// </summary>
        public abstract void Wait();
    }
}
