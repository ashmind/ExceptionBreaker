using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace ExceptionBreaker.Options.Support {
    public static class ObservableCollectionExtensions {
        public static IObservableResult<TResult> GetObservable<TCollection, TResult>(this TCollection collection, Func<TCollection, TResult> get, Action<TCollection, NotifyCollectionChangedEventArgs, Action> subscribeExtra = null)
            where TCollection : INotifyCollectionChanged 
        {
            subscribeExtra = subscribeExtra ?? ((c, e, a) => {});
            var result = new ObservableValue<TResult>(get(collection));
            collection.CollectionChanged += (sender, e) => {
                result.Value = get(collection);
                subscribeExtra(collection, e, () => result.Value = get(collection));
            };

            return result;
        }

        public static void AddHandlers<T>(this ObservableCollection<T> collection, Action<T> onAdded, Action<T> onRemoved) {
            foreach (var item in collection) {
                onAdded(item);
            }
            collection.CollectionChanged += (sender, e) => e.ProcessChanges(onAdded, onRemoved);
        }

        public static void ProcessChanges<T>(this NotifyCollectionChangedEventArgs e, Action<T> processNew, Action<T> processOld) {
            if (e.NewItems != null) {
                foreach (T item in e.NewItems) {
                    processNew(item);
                }
            }

            if (e.OldItems != null) {
                foreach (T item in e.OldItems) {
                    processOld(item);
                }
            }
        }
    }
}
