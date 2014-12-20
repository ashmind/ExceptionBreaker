using System.Text.RegularExpressions;
using System.Windows.Data;
using ExceptionBreaker.Options.Support;

namespace ExceptionBreaker.Options {
    public class PatternViewModel {
        public PatternViewModel(PatternData data) {
            Data = data;
            Pattern = new ObservableValue<string>(data.Regex.ToString());
            Enabled = new ObservableValue<bool>(data.Enabled);
            IsEmpty = Pattern.GetObservable(string.IsNullOrEmpty);

            Enabled.PropertyChanged += (sender, e) => {
                data.Enabled = ((ObservableValue<bool>)sender).Value;
            };
            Pattern.PropertyChanged += (sender, e) => {
                var pattern = ((ObservableValue<string>) sender).Value;
                if (string.IsNullOrEmpty(pattern))
                    return;
            
                data.Regex = new Regex(pattern);
            };

        }

        public PatternViewModel(string pattern) : this(new PatternData(new Regex(pattern))) {
        }

        public PatternData Data { get; private set; }
        public ObservableValue<string> Pattern { get; private set; }
        public ObservableValue<bool> Enabled { get; private set; }
        public IObservableResult<bool> IsEmpty { get; private set; }
    }
}