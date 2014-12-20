using System.Text.RegularExpressions;
using ExceptionBreaker.Options.Support;

namespace ExceptionBreaker.Options {
    public class PatternViewModel {
        public PatternViewModel() {
            Value = new ObservableValue<string>("");
            IsEmpty = Value.GetObservable(string.IsNullOrEmpty);
        }

        public PatternViewModel(Regex regex) : this() {
            Value.Value = regex.ToString();
        }

        public ObservableValue<string> Value { get; private set; }
        public IObservableResult<bool> IsEmpty { get; private set; }

        public Regex ToRegex() {
            return new Regex(Value.Value);
        }
    }
}