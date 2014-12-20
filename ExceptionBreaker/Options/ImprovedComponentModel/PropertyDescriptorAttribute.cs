using System;
using System.Collections.Generic;

namespace ExceptionBreaker.Options.ImprovedComponentModel {
    public class PropertyDescriptorAttribute : Attribute {
        public Type PropertyDescriptorType { get; private set; }

        public PropertyDescriptorAttribute(Type propertyDescriptorType) {
            PropertyDescriptorType = propertyDescriptorType;
        }
    }
}
