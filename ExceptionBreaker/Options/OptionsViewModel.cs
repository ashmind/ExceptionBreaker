using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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

            AllExceptions.PropertyChanged += (sender, e) => RecalculateExceptionsMatchingIgnored();
            
            IgnoredPatterns.Values.CollectionChanged += (sender, e) => RecalculateExceptionsMatchingIgnored();
            var ignoredChangeHandler = (PropertyChangedEventHandler)((_, __) => RecalculateExceptionsMatchingIgnored());
            IgnoredPatterns.Values.AddHandlers(
                added => added.Value.PropertyChanged += ignoredChangeHandler,
                removed => removed.Value.PropertyChanged -= ignoredChangeHandler
            );

            RecalculateExceptionsMatchingIgnored();
        }

        private void RecalculateExceptionsMatchingIgnored() {
            AllExceptions.Value.SyncToWhere(_exceptionsMatchingIgnored, e => _data.Ignored.Any(r => r.IsMatch(e.Name)));
        }

        public PatternCollectionViewModel IgnoredPatterns { get; private set; }
        public IObservableResult<ReadOnlyCollection<ExceptionViewModel>> AllExceptions { get; private set; }
        public ReadOnlyObservableCollection<ExceptionViewModel> ExceptionsMatchingIgnored { get; private set; }
    }
}
