using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ExceptionBreaker.Options {
    public class FailSafeJsonTypeConverter : TypeConverter {
        private readonly Type _targetType;

        public FailSafeJsonTypeConverter(Type targetType) {
            _targetType = targetType;
        }

        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings {
            Formatting = Formatting.None,
            Converters = { new StringEnumConverter() }
        };

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
            var stringValue = value as string;
            if (stringValue != null) {
                try {
                    return JsonConvert.DeserializeObject(stringValue, _targetType, JsonSettings);
                }
                catch (JsonException) {
                    return null;
                }
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
            return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
            if (destinationType == typeof (string))
                return JsonConvert.SerializeObject(value, JsonSettings);

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}