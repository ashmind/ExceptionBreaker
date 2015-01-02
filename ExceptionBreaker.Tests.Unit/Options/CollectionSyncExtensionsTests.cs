using System;
using System.Collections.Generic;
using System.Linq;
using ExceptionBreaker.Options.Support;
using Xunit;
using Xunit.Extensions;

namespace ExceptionBreaker.Tests.Unit.Options {
    public class CollectionSyncExtensionsTests {
        [Theory]
        [InlineData("1*,2,3*", "", "1*,3*")]
        [InlineData("1,2*,3",  "", "2*")]
        [InlineData("1,2,3",   "", "")]
        [InlineData("1,2*,3*,4", "2*,3*,4", "2*,3*")]
        [InlineData("1*,2,3",  "2,3", "1*")]
        [InlineData("1*,2,3",  "", "1*")]
        [InlineData("1,2,3",   "1,2,3", "")]
        public void SyncToWhere_ProducesCorrectCollection(string source, string target, string expected) {
            var targetList = SplitToList(target);
            SplitToList(source).SyncToWhere(targetList, s => s.EndsWith("*"));

            Assert.Equal(SplitToList(expected), targetList);
        }

        private static IList<string> SplitToList(string target) {
            return target.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }
    }
}
