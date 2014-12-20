using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ExceptionBreaker.Options.ImprovedComponentModel {
    public class ImprovedTypeDescriptor : CustomTypeDescriptor {
        public ImprovedTypeDescriptor(ICustomTypeDescriptor parent) : base(parent) {
        }

        public override PropertyDescriptorCollection GetProperties() {
            return ProcessProperties(base.GetProperties());
        }

        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes) {
            return ProcessProperties(base.GetProperties(attributes));
        }

        private static PropertyDescriptorCollection ProcessProperties(PropertyDescriptorCollection properties) {
            var newList = new List<PropertyDescriptor>();
            foreach (PropertyDescriptor property in properties) {
                var descriptorAttribute = (PropertyDescriptorAttribute)property.Attributes[typeof(PropertyDescriptorAttribute)];
                if (descriptorAttribute != null) {
                    var newProperty = (PropertyDescriptor)Activator.CreateInstance(descriptorAttribute.PropertyDescriptorType, new[] { property });
                    newList.Add(newProperty);
                    continue;
                }
                newList.Add(property);
            }
            return new PropertyDescriptorCollection(newList.ToArray());
        }
    }
}