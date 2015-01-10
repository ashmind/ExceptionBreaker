using System;
using ExceptionBreaker.Core.Observable;
using ExceptionBreaker.Options.Support;

namespace ExceptionBreaker.Options {
    public class ExceptionViewModel {
        public ExceptionViewModel(string name, ObservableValue<PatternViewModel> selected) {
            Name = name;
            MatchesSelected = selected.GetObservable(
                s => s != null && !s.IsEmpty.Value && s.Data.Matches(name),
                (newItem, oldItem, changed) => {
                    var handler = (EventHandler)delegate { changed(); };
                    if (oldItem != null) {
                        oldItem.Pattern.ValueChanged -= handler;
                        oldItem.Enabled.ValueChanged -= handler;
                    }

                    if (newItem != null) {
                        newItem.Pattern.ValueChanged += handler;
                        newItem.Enabled.ValueChanged += handler;
                    }
                }
            );
        }

        public string Name { get; private set; }
        public IObservableResult<bool> MatchesSelected { get; private set; } 
    }
}