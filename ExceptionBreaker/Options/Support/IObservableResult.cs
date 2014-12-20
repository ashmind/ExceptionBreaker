using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ExceptionBreaker.Options.Support {
    public interface IObservableResult<T> : INotifyPropertyChanged {
        T Value { get; }
        IObservableResult<TResult> GetObservable<TResult>(Func<T, TResult> get, Action<T, T, Action> subscribeExtra = null);
    }
}
