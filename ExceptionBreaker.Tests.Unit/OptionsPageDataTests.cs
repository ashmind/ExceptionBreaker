using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AshMind.Extensions;
using ExceptionBreaker.Options;
using Microsoft.VisualStudio.Shell.Interop;
using Moq;
using Xunit;

namespace ExceptionBreaker.Tests.Unit {
    public class OptionsPageDataTests {
        [Fact]
        public void XmlSettings_CanBeReloaded() {
            var data = new OptionsPageData {
                Ignored = { new PatternData(new Regex("test")) }
            };

            var settings = new Dictionary<string, string>();
            var writer = MockDictionarySettingsWriter(settings);
            data.SaveSettingsToXml(writer.Object);

            var reloaded = new OptionsPageData();
            var reader = MockDictionarySettingsReader(settings);
            reloaded.LoadSettingsFromXml(reader.Object);

            Assert.Equal(
                data.Ignored.Select(p => new { Regex = p.Regex.ToString(), p.Enabled }),
                reloaded.Ignored.Select(p => new { Regex = p.Regex.ToString(), p.Enabled })
            );
        }

        private static Mock<IVsSettingsWriter> MockDictionarySettingsWriter(IDictionary<string, string> settings) {
            var writer = new Mock<IVsSettingsWriter>();
            writer.Setup(x => x.WriteSettingString(It.IsAny<string>(), It.IsAny<string>()))
                .Callback((string key, string value) => settings.Add(key, value));
            return writer;
        }

        private Mock<IVsSettingsReader> MockDictionarySettingsReader(IDictionary<string, string> settings) {
            var reader = new Mock<IVsSettingsReader>();
            string _;
            reader.Setup(x => x.ReadSettingString(It.IsAny<string>(), out _))
                  .Callback((string key, out string value) => { value = settings.GetValueOrDefault(key); });
            return reader;
        }
    }
}
