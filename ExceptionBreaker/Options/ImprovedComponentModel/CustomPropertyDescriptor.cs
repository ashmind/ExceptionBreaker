using System;
using System.ComponentModel;

namespace ExceptionBreaker.Options.ImprovedComponentModel {
    public class CustomPropertyDescriptor : PropertyDescriptor {
        private readonly PropertyDescriptor _parent;

        public CustomPropertyDescriptor(PropertyDescriptor parent) : base(parent) {
            _parent = parent;
        }

        public override void AddValueChanged(object component, EventHandler handler) {
            _parent.AddValueChanged(component, handler);
        }

        public override bool CanResetValue(object component) {
            return _parent.CanResetValue(component);
        }

        public override PropertyDescriptorCollection GetChildProperties(object instance, Attribute[] filter) {
            return _parent.GetChildProperties(instance, filter);
        }

        public override object GetEditor(Type editorBaseType) {
            return _parent.GetEditor(editorBaseType);
        }

        public override object GetValue(object component) {
            return _parent.GetValue(component);
        }

        public override void RemoveValueChanged(object component, EventHandler handler) {
            _parent.RemoveValueChanged(component, handler);
        }

        public override void ResetValue(object component) {
            _parent.ResetValue(component);
        }

        public override void SetValue(object component, object value) {
            _parent.SetValue(component, value);
        }

        public override bool ShouldSerializeValue(object component) {
            return _parent.ShouldSerializeValue(component);
        }

        public override Type ComponentType {
            get { return _parent.ComponentType; }
        }

        public override TypeConverter Converter {
            get { return _parent.Converter; }
        }

        public override bool IsLocalizable {
            get { return _parent.IsLocalizable; }
        }

        public override bool IsReadOnly {
            get { return _parent.IsReadOnly; }
        }

        public override Type PropertyType {
            get { return _parent.PropertyType; }
        }

        public override bool SupportsChangeEvents {
            get { return _parent.SupportsChangeEvents; }
        }

        public override string DisplayName {
            get { return _parent.DisplayName; }
        }

        public override bool DesignTimeOnly {
            get { return _parent.DesignTimeOnly; }
        }

        public override string Name {
            get { return _parent.Name; }
        }

        public override bool IsBrowsable {
            get { return _parent.IsBrowsable; }
        }

        public override string Description {
            get { return _parent.Description; }
        }

        public override string Category {
            get { return _parent.Category; }
        }

        public override AttributeCollection Attributes {
            get { return _parent.Attributes; }
        }
    }
}