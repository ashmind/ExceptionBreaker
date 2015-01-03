using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace ExceptionBreaker.Breakpoints.Glyphs {
    [Export(typeof(IGlyphFactoryProvider))]
    [Name(BreakpointGlyphFactory.Name)]
    [Order(After = "VsTextMarker")]
    [ContentType("code")]
    [TextViewRole(PredefinedTextViewRoles.Debuggable)]
    [TagType(typeof(BreakpointTag))]
    public class BreakpointGlyphFactoryProvider : IGlyphFactoryProvider {
        public IGlyphFactory GetGlyphFactory(IWpfTextView view, IWpfTextViewMargin margin) {
            return new BreakpointGlyphFactory(margin);
        }
    }
}