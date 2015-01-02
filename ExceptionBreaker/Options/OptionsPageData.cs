using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using AshMind.Extensions;
using ExceptionBreaker.Core;
using ExceptionBreaker.Options.ImprovedComponentModel;
using ExceptionBreaker.Options.Support;
using Microsoft.Forums.WpfDialogPageIntegration;

namespace ExceptionBreaker.Options {
    public class OptionsPageData : UIElementDialogPage {
        private readonly Lazy<IExceptionListProvider> _exceptionListProvider;
        private readonly Lazy<IDiagnosticLogger> _logger;

        static OptionsPageData() {
            ImprovedTypeDescriptorProvider.RegisterFor(typeof(OptionsPageData));
        }

        public OptionsPageData() : this(
            new Lazy<IExceptionListProvider>(() => ExceptionBreakerPackage.Current.ExceptionBreakManager),
            new Lazy<IDiagnosticLogger>(() => ExceptionBreakerPackage.Current.Logger)
        ) {}

        public OptionsPageData(Lazy<IExceptionListProvider> exceptionListProvider, Lazy<IDiagnosticLogger> logger) {
            _exceptionListProvider = exceptionListProvider;
            _logger = logger;
            Ignored = new List<PatternData>();
        }
        
        [TypeConverter(typeof(FailSafeJsonTypeConverter))]
        [PropertyDescriptor(typeof(ProperListPropertyDescriptor))]
        public IList<PatternData> Ignored { get; private set; }

        protected override UIElement Child {
            get { return CreateChild(); }
        }

        private UIElement CreateChild() {
            var manager = _exceptionListProvider.Value;
            var exceptionNamesObservable = new ObservableValue<IEnumerable<string>>(manager.GetExceptionNames());
            _exceptionListProvider.Value.ExceptionNamesChanged += delegate {
                var count = 0;
                exceptionNamesObservable.Value = manager.GetExceptionNames()
                    .OnAfterEach(_ => count += 1)
                    .OnAfterLast(_ => _logger.Value.WriteLine("Options: Found {0} exceptions for preview.", count));
            };

            return new OptionsPageView {
                ViewModel = new OptionsViewModel(this, exceptionNamesObservable)
            };
        }
    }
}
