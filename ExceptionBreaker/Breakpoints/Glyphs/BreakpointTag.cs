using Microsoft.VisualStudio.Text.Editor;

namespace ExceptionBreaker.Breakpoints.Glyphs {
    public class BreakpointTag : IGlyphTag {
        public BreakpointExtraData BreakpointExtraData { get; private set; }

        public BreakpointTag(BreakpointExtraData breakpointExtraData) {
            BreakpointExtraData = breakpointExtraData;
        }
    }
}
