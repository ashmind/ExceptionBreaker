using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using ExceptionBreaker.Options.ImprovedComponentModel;

namespace ExceptionBreaker.Options.Support {
    public class ProperListPropertyDescriptor : CustomPropertyDescriptor {
        public ProperListPropertyDescriptor(PropertyDescriptor parent) : base(parent) {
        }

        public override bool IsReadOnly {
            get { return false; }
        }

        public override void SetValue(object component, object value) {
            var list = (IList)base.GetValue(component);
            var newList = (IEnumerable)value;

            list.Clear();
            if (newList == null)
                return;

            foreach (var item in newList) {
                list.Add(item);
            }
        }
    }
}
