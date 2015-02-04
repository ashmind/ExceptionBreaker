using System;
using System.ComponentModel;
using System.Windows.Controls;

namespace ExceptionBreaker.Core.Observable {
    public class ObservableValue<T> : IObservableResult<T> {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public event EventHandler ValueChanged = delegate { };
        private static readonly PropertyChangedEventArgs PropertyChangedEventArgs = new PropertyChangedEventArgs("Value");

        private T _value;

        public ObservableValue() {
        }

        public ObservableValue(T value) {
            _value = value;
        }
        
        public T Value {
            get { return _value; }
            set {
                if (Equals(_value, value))
                    return;

                _value = value;
                PropertyChanged(this, PropertyChangedEventArgs);
                ValueChanged(this, EventArgs.Empty);
            }
        }

        public IObservableResult<TResult> GetObservable<TResult>(Func<T, TResult> get, Action<T, T, Action> subscribeExtra = null) {
            var result = new ObservableValue<TResult>(get(Value));
            var lastValue = Value;
            PropertyChanged += (sender, _) => {
                var that = (ObservableValue<T>)sender;
                result.Value = get(that.Value);
                if (subscribeExtra != null)
                    subscribeExtra(that.Value, lastValue, () => result.Value = get(that.Value));
                lastValue = Value;
            };
            return result;
        }

        public override string ToString() {
            return Value != null ? Value.ToString() : string.Empty;
        }
    }
}