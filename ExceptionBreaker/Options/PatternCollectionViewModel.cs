using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Linq;
using AshMind.Extensions;
using ExceptionBreaker.Options.Support;

namespace ExceptionBreaker.Options {
    public class PatternCollectionViewModel {
        public PatternCollectionViewModel(ICollection<Regex> data) {
            Values = new ObservableCollection<PatternViewModel>(data.Select(r => new PatternViewModel(r)));
            Values.CollectionChanged += (sender, e) => {
                UpdateData(data);
                var handler = (PropertyChangedEventHandler)((_, __) => UpdateData(data));
                e.ProcessChanges<PatternViewModel>(
                    newItem => newItem.Value.PropertyChanged += handler,
                    oldItem => oldItem.Value.PropertyChanged -= handler
                );
            };

            Selected = new ObservableValue<PatternViewModel>();

            CanAdd = Values.GetObservable(c => c.All(v => !v.IsEmpty.Value), (c, e, changed) => {
                var handler = (PropertyChangedEventHandler)delegate { changed(); };
                e.ProcessChanges<PatternViewModel>(
                    newItem => newItem.IsEmpty.PropertyChanged += handler,
                    oldItem => oldItem.IsEmpty.PropertyChanged -= handler
                );
            });
            CanDelete = Selected.GetObservable(v => v != null);
        }

        private void UpdateData(ICollection<Regex> data) {
            data.Clear();
            data.AddRange(Values.Where(m => !m.IsEmpty.Value).Select(m => m.ToRegex()));
        }

        public ObservableCollection<PatternViewModel> Values { get; private set; }
        public IObservableResult<bool> CanAdd { get; private set; }
        public IObservableResult<bool> CanDelete { get; private set; }
        public ObservableValue<PatternViewModel> Selected { get; private set; }

        public void AddNew() {
            Values.Add(new PatternViewModel());
        }

        public void DeleteSelected() {
            Values.Remove(Selected.Value);
            Selected.Value = null;
        }
    }
}
