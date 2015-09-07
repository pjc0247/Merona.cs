using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merona
{
    public class SafeCollection<T> : ObservableCollection<T>
        where T : ISafeCollectionContainable<T>
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
                    ((ISafeCollectionContainable<T>)e.NewItems[0]).OnAdded(this);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    ((ISafeCollectionContainable<T>)e.OldItems[0]).OnAdded(this);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    ((ISafeCollectionContainable<T>)e.NewItems[0]).OnAdded(this);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    foreach(var item in e.OldItems)
                        ((ISafeCollectionContainable<T>)item).OnRemoved(this);
                    break;
            }
        }

        public void Invalidate(T item)
        {
            Remove(item);
        }
    }

    public interface ISafeCollectionContainable<T>
        where T : ISafeCollectionContainable<T>
    {
        void OnAdded(SafeCollection<T> safeCollection);
        void OnRemoved(SafeCollection<T> safeCollection);
    }
}
