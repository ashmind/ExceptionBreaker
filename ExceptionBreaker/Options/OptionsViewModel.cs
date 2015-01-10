using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using ExceptionBreaker.Core.Observable;
using ExceptionBreaker.Options.Support;

namespace ExceptionBreaker.Options {
    public class OptionsViewModel {
        private readonly OptionsPageData _data;
        private readonly ObservableCollection<ExceptionViewModel> _exceptionsMatchingIgnored;

        public OptionsViewModel(OptionsPageData data,  ObservableValue<IEnumerable<string>> exceptionNames) {
            _data = data;
            IgnoredPatterns = new PatternCollectionViewModel(data.Ignored);
            AllExceptions = exceptionNames.GetObservable(v => new ReadOnlyCollection<ExceptionViewModel>(
                v.Select(n => new ExceptionViewModel(n, IgnoredPatterns.Selected)).ToArray()
            ));

            _exceptionsMatchingIgnored = new ObservableCollection<ExceptionViewModel>();
            ExceptionsMatchingIgnored = new ReadOnlyObservableCollection<ExceptionViewModel>(_exceptionsMatchingIgnored);

            AllExceptions.ValueChanged += (sender, e) => RecalculateExceptionsMatchingIgnored();
            
            IgnoredPatterns.Values.CollectionChanged += (sender, e) => RecalculateExceptionsMatchingIgnored();
            var ignoredChangeHandler = (EventHandler) delegate { RecalculateExceptionsMatchingIgnored(); };
            IgnoredPatterns.Values.AddHandlers(
                added => {
                    added.Pattern.ValueChanged += ignoredChangeHandler;
                    added.Enabled.ValueChanged += ignoredChangeHandler;
                },
                removed => {
                    removed.Pattern.ValueChanged -= ignoredChangeHandler;
                    removed.Enabled.ValueChanged -= ignoredChangeHandler;
                }
            );

            RecalculateExceptionsMatchingIgnored();
        }

        private void RecalculateExceptionsMatchingIgnored() {
            AllExceptions.Value.SyncToWhere(_exceptionsMatchingIgnored, e => _data.Ignored.Any(p => p.Matches(e.Name)));
        }

        public PatternCollectionViewModel IgnoredPatterns { get; private set; }
        public IObservableResult<ReadOnlyCollection<ExceptionViewModel>> AllExceptions { get; private set; }
        public ReadOnlyObservableCollection<ExceptionViewModel> ExceptionsMatchingIgnored { get; private set; }
    }
}
