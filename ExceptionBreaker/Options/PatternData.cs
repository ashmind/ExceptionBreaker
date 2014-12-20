using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ExceptionBreaker.Options {
    public class PatternData {
        private Regex _regex;

        public PatternData(Regex regex) {
            Argument.NotNull("regex", regex);
            Regex = regex;
            Enabled = true;
        }

        public bool Enabled { get; set; }
        public Regex Regex {
            get { return _regex; }
            set { _regex = Argument.NotNull("value", value); }
        }

        public bool Matches(string name) {
            return Enabled && Regex.IsMatch(name);
        }
    }
}
