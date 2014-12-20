using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ExceptionBreaker.Options;
using Xunit;

namespace ExceptionBreaker.Tests.Unit {
    public class PatternCollectionViewModelTests {
        [Fact]
        public void Add_AddsItemToData_IfPatternIsSet() {
            var data = new Collection<PatternData>();
            var model = new PatternCollectionViewModel(data);
            model.Values.Add(new PatternViewModel("x"));

            Assert.Equal(new[] { "x" }, data.Select(p => p.Regex.ToString()));
        }

        [Fact]
        public void Add_DoesNotAddItemToData_IfPatternIsNotSet() {
            var data = new Collection<PatternData>();
            var model = new PatternCollectionViewModel(data);
            model.Values.Add(new PatternViewModel(""));

            Assert.Empty(data);
        }

        [Fact]
        public void PatternChange_AddsItemToData_IfPatternIsSet() {
            var data = new Collection<PatternData>();
            var model = new PatternCollectionViewModel(data);
            var pattern = new PatternViewModel("");
            model.Values.Add(pattern);

            pattern.Pattern.Value = "x";
            Assert.Equal(new[] { "x" }, data.Select(p => p.Regex.ToString()));
        }

        [Fact]
        public void PatternChange_RemovesItemToData_IfPatternIsNotSet() {
            var data = new Collection<PatternData>();
            var model = new PatternCollectionViewModel(data);
            var pattern = new PatternViewModel("x");
            model.Values.Add(pattern);

            pattern.Pattern.Value = "";
            Assert.Empty(data);
        }
    }
}
