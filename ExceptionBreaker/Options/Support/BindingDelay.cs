using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

namespace ExceptionBreaker.Options.Support {
    public class BindingDelay {
        private readonly DependencyProperty _property;
        private readonly DispatcherTimer _timer;
        private DependencyObject _target;
        private BindingExpression _bindingExpression;

        public BindingDelay(DependencyProperty property, TimeSpan delay) {
            _property = property;

            _timer = new DispatcherTimer();
            _timer.Tick += Timer_Tick;
            _timer.Interval = delay;
        }

        private void Timer_Tick(object sender, EventArgs eventArgs) {
            _timer.Stop();
            _bindingExpression.UpdateSource();
        }

        private void Set(DependencyObject target) {
            var element = target as UIElement;
            if (element == null) {
                SetImmediate(target);
                return;
            }

            EventHandler handler = null;
            handler = ((sender, e) => {
                element.LayoutUpdated -= handler;
                SetImmediate(element);
            });
            element.LayoutUpdated += handler;
        }

        private void SetImmediate(DependencyObject target) {
            _target = target;
            _bindingExpression = BindingOperations.GetBindingExpression(target, _property);
            if (_bindingExpression == null)
                throw new InvalidOperationException("Binding not found on " + target + " " + _property.Name + ".");

            if (_bindingExpression.ParentBinding.UpdateSourceTrigger != UpdateSourceTrigger.Explicit)
                throw new InvalidOperationException("Binding UpdateSourceTrigger must be set to Explicit.");

            var descriptor = DependencyPropertyDescriptor.FromProperty(_property, target.GetType());
            descriptor.AddValueChanged(target, (sender, e) => {
                _timer.Stop();
                _timer.Start();
            });
        }

        public static void SetList(DependencyObject target, ICollection<BindingDelay> delays) {
            foreach (var delay in delays) {
                delay.Set(target);
            }
        }
    }
}
