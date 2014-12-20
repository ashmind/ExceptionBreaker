using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ExceptionBreaker.Options.ImprovedComponentModel {
    public class ImprovedTypeDescriptorProvider : TypeDescriptionProvider {
        public ImprovedTypeDescriptorProvider(TypeDescriptionProvider parent) : base(parent) {
        }

        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance) {
            return new ImprovedTypeDescriptor(base.GetTypeDescriptor(objectType, instance));
        }

        public static void RegisterFor(Type type) {
            var provider = TypeDescriptor.GetProvider(type);
            if (provider is ImprovedTypeDescriptorProvider)
                return;

            TypeDescriptor.AddProvider(new ImprovedTypeDescriptorProvider(provider), type);
        }
    }
}
