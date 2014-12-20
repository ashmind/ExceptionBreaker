using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using AshMind.Extensions;
using ExceptionBreaker.Options.Support;

namespace ExceptionBreaker.Options {
    public class PatternCollectionViewModel {
        private readonly ICollection<PatternData> _data;

        public PatternCollectionViewModel(ICollection<PatternData> data) {
            Argument.NotNull("data", data);
            _data = data;

            Values = new ObservableCollection<PatternViewModel>(data.Select(d => new PatternViewModel(d)));
            SetupValues();

            Selected = new ObservableValue<PatternViewModel>();

            CanAdd = Values.GetObservable(c => c.All(v => !v.IsEmpty.Value), (c, e, changed) => {
                var handler = (EventHandler)delegate { changed(); };
                e.ProcessChanges<PatternViewModel>(
                    newItem => newItem.IsEmpty.ValueChanged += handler,
                    oldItem => oldItem.IsEmpty.ValueChanged -= handler
                );
            });
            CanDelete = Selected.GetObservable(v => v != null);
        }

        private void SetupValues() {
            Values.CollectionChanged += (sender, e) => UpdateData();
            var valuesChangedHandler = (EventHandler)delegate { UpdateData(); };
            Values.AddHandlers(
                added => added.IsEmpty.ValueChanged += valuesChangedHandler,
                removed => removed.IsEmpty.ValueChanged -= valuesChangedHandler
            );
        }

        private void UpdateData() {
            _data.Clear();
            _data.AddRange(Values.Where(v => !v.IsEmpty.Value).Select(v => v.Data));
        }

        public ObservableCollection<PatternViewModel> Values { get; private set; }
        public IObservableResult<bool> CanAdd { get; private set; }
        public IObservableResult<bool> CanDelete { get; private set; }
        public ObservableValue<PatternViewModel> Selected { get; private set; }

        public void AddNew() {
            Values.Add(new PatternViewModel(""));
        }

        public void DeleteSelected() {
            Values.Remove(Selected.Value);
            Selected.Value = null;
        }
    }
}
