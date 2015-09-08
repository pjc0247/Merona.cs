using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merona
{
    public class SafeCollection<T> : ObservableCollection<T>, IStatusSubscriber<T>
        where T : IStatusObservable<T>
    {
        public SafeCollection() :
            base()
        {
            CollectionChanged += SafeCollection_CollectionChanged;
        }
        public SafeCollection(T[] values) :
            base(values)
        {
            CollectionChanged += SafeCollection_CollectionChanged;
        }

        private void SafeCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    ((IStatusObservable<T>)e.NewItems[0]).OnSubscribe(this);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    ((IStatusObservable<T>)e.OldItems[0]).OnSubscribe(this);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    ((IStatusObservable<T>)e.NewItems[0]).OnSubscribe(this);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    foreach(var item in e.OldItems)
                        ((IStatusObservable<T>)item).OnUnsubscribe(this);
                    break;
            }
        }

        public void Invalidate(T item)
        {
            Remove(item);
        }
    }

    public interface IStatusObservable<T>
        where T : IStatusObservable<T>
    {
        void OnSubscribe(IStatusSubscriber<T> obj);
        void OnUnsubscribe(IStatusSubscriber<T> obj);
    }
    public interface IStatusSubscriber<T>
        where T : IStatusObservable<T>
    {
        void Invalidate(T obj);
    }
}
