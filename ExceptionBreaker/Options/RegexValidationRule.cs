using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace ExceptionBreaker.Options {
    public class RegexValidationRule : ValidationRule {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo) {
            var @string = (string)value;
            try {
                new Regex(@string);
            }
            catch (ArgumentException ex) {
                // https://connect.microsoft.com/VisualStudio/feedback/details/331753/regex-should-have-a-static-tryparse-method
                return new ValidationResult(false, ex.Message);
            }
            return ValidationResult.ValidResult;
        }
    }
}
