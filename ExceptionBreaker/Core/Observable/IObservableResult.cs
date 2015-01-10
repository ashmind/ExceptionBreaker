using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ExceptionBreaker.Core.Observable {
    public interface IObservableResult<T> : INotifyPropertyChanged {
        event EventHandler ValueChanged;
        T Value { get; }
        IObservableResult<TResult> GetObservable<TResult>(Func<T, TResult> get, Action<T, T, Action> subscribeExtra = null);
    }
}
