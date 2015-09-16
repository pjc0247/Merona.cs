using System;
using System.Collections.Generic;
using System.Linq;

namespace Merona
{
    public interface IStatusObservable<T>
        where T : IStatusObservable<T>
    {
        void OnSubscribe(IStatusSubscriber<T> obj);
        void OnUnsubscribe(IStatusSubscriber<T> obj);
    }
    public interface IStatusSubscriber<T>
        where T : IStatusObservable<T>
    {
        void OnInvalidate(T obj);
    }
}
