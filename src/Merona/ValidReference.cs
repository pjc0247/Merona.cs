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

        public T Get()
        {
            if (isValid == false)
                throw new InvalidOperationException();

            return target;
        }
    }
}
