using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Media.TextFormatting;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace ExceptionBreaker.Breakpoints.Glyphs {
    [Export(typeof(ITaggerProvider))]
    [Order(After = "VsTextMarker")]
    [ContentType("code")]
    [TextViewRole(PredefinedTextViewRoles.Debuggable)]
    [TagType(typeof(BreakpointTag))]
    public class BreakpointTaggerProvider : ITaggerProvider {
        private readonly BreakpointFinder _finder;
        private readonly BreakpointExtraDataStore _extraDataStore;

        [ImportingConstructor]
        public BreakpointTaggerProvider(BreakpointFinder finder, BreakpointExtraDataStore extraDataStore) {
            _finder = finder;
            _extraDataStore = extraDataStore;
        }

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag {
            var document = (ITextDocument)buffer.Properties[typeof(ITextDocument)];
            return (ITagger<T>)new BreakpointTagger(document, _finder, _extraDataStore);
        }
    }
}
