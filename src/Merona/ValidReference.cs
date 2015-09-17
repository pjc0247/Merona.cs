using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merona
{
    public class ValidReference<T> : IStatusSubscriber<T>
        where T : IStatusObservable<T>
    {
        private T target { get; set; }

        public bool isValid { get; private set; }

        public ValidReference(T target)
        {
            this.target = target;
            this.isValid = true;

            target.OnSubscribe(this);
        }

        public void OnInvalidate(T item)
        {
            isValid = false;
        }

        /// <summary>
        /// 가리키고 있는 객체를 얻어온다.
        /// 만약 가리키고 있는 객체가 더이상 유효하지 않을 경우
        /// 익셉션을 발생시킨다.
        /// </summary>
        /// <returns>유효할 경우 가리키는 객체</returns>
        /// <exception cref="InvalidOperationException">
        /// 가리키는 객체가 더 이상 유효하지 않을 경우
        /// </exception>
        public T Get()
        {
            if (isValid == false)
                throw new InvalidOperationException();

            return target;
        }
        public bool TryGet(out T obj)
        {
            if (isValid == false)
            {
                obj = default(T);
                return false;
            }

            obj = target;
            return true;
        }
    }
}
