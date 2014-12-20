using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CsvHelper;
using CsvHelper.Configuration;

namespace ExceptionBreaker.Options {
    public class RegexListTypeConverter : TypeConverter {
        private static readonly CsvConfiguration CsvConfiguration = new CsvConfiguration {
            HasHeaderRecord = false,
            QuoteAllFields = true
        };

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
            var stringValue = value as string;
            if (stringValue != null)
                return ConvertFromStringInternal(stringValue);

            return base.ConvertFrom(context, culture, value);
        }

        private IList<Regex> ConvertFromStringInternal(string value) {
            if (value.Length == 0)
                return new List<Regex>();

            var reader = new CsvReader(new StringReader(value), CsvConfiguration);
            reader.Read();
            return reader.CurrentRecord.Select(s => new Regex(s)).ToList();
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
            return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
            if (destinationType == typeof (string))
                return ConvertToStringInternal((IList<Regex>)value);

            return base.ConvertTo(context, culture, value, destinationType);
        }

        private object ConvertToStringInternal(IEnumerable<Regex> value) {
            var stringWriter = new StringWriter();
            var writer = new CsvWriter(stringWriter, CsvConfiguration);
            foreach (var regex in value) {
                writer.WriteField(regex.ToString());
            }
            writer.NextRecord();
            return stringWriter.ToString().TrimEnd('\r', '\n');
        }
    }
}