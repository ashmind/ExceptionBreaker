using System.ComponentModel;
using ExceptionBreaker.Options.Support;

namespace ExceptionBreaker.Options {
    public class ExceptionViewModel {
        public ExceptionViewModel(string name, ObservableValue<PatternViewModel> selected) {
            Name = name;
            MatchesSelected = selected.GetObservable(
                s => s != null && !s.IsEmpty.Value && s.ToRegex().IsMatch(name),
                (newItem, oldItem, changed) => {
                    var handler = (PropertyChangedEventHandler) delegate { changed(); };
                    if (oldItem != null)
                        oldItem.Value.PropertyChanged -= handler;

                    if (newItem != null)
                        newItem.Value.PropertyChanged += handler;
                }
            );
        }

        public string Name { get; private set; }
        public IObservableResult<bool> MatchesSelected { get; private set; } 
    }
}