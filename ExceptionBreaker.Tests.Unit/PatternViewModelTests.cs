using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ExceptionBreaker.Options;
using Xunit;

namespace ExceptionBreaker.Tests.Unit {
    public class PatternViewModelTests {
        [Fact]
        public void Pattern_IsSetFromDataRegex() {
            var model = new PatternViewModel(new PatternData(new Regex("x")));
            Assert.Equal("x", model.Pattern.Value);
        }

        [Fact]
        public void DataRegex_IsUpdatedWhenPatternIsSet() {
            var model = new PatternViewModel("");
            model.Pattern.Value = "x";
            Assert.Equal("x", model.Data.Regex.ToString());
        }

        [Fact]
        public void Enabled_IsSetFromDataEnabled() {
            var model = new PatternViewModel(new PatternData(new Regex("")) { Enabled = false });
            Assert.False(model.Enabled.Value);
        }

        [Fact]
        public void DataEnabled_IsUpdatedWhenEnabledIsSet() {
            var model = new PatternViewModel("");
            model.Enabled.Value = !model.Enabled.Value;
            Assert.Equal(model.Enabled.Value, model.Data.Enabled);
        }

        [Fact]
        public void IsEmpty_IsTrue_IfPatternIsSetToEmpty() {
            var model = new PatternViewModel("x");
            model.Pattern.Value = "";

            Assert.True(model.IsEmpty.Value);
        }

        [Fact]
        public void IsEmpty_IsFalse_IfPatternIsSetToNonEmpty() {
            var model = new PatternViewModel("");
            model.Pattern.Value = "x";

            Assert.False(model.IsEmpty.Value);
        }
    }
}
