using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;

namespace ExceptionBreaker.Options.Support {
    public class BindingDelayExtension : MarkupExtension {
        public override object ProvideValue(IServiceProvider serviceProvider) {
            return new[] { new BindingDelay(Property, TimeSpan.FromMilliseconds(Delay)) };
        }

        public DependencyProperty Property { get; set; }
        public int Delay { get; set; }
    }
}
