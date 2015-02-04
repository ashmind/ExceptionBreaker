using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace ExceptionBreaker.Breakpoints.Glyphs {
    public class BreakpointTagger : ITagger<BreakpointTag> {
        private readonly ITextDocument _document;
        private readonly BreakpointFinder _finder;
        private readonly BreakpointExtraDataStore _extraDataStore;

        public BreakpointTagger(ITextDocument document, BreakpointFinder finder, BreakpointExtraDataStore extraDataStore) {
            _document = document;
            _finder = finder;
            _extraDataStore = extraDataStore;
            _extraDataStore.DataChanged += (sender, e) => {
                var handler = TagsChanged;
                if (handler != null)
                    handler(this, new SnapshotSpanEventArgs(finder.GetSpanFromBreakpoint(e.Breakpoint, document)));
            };
        }

        public IEnumerable<ITagSpan<BreakpointTag>> GetTags(NormalizedSnapshotSpanCollection spans) {
            foreach (var span in spans) {
                var breakpoint = _finder.GetBreakpointFromSpan(span, _document);
                if (breakpoint == null)
                    continue;

                var extraData = _extraDataStore.GetData(breakpoint);
                if (extraData.ExceptionBreakChange.Value == ExceptionBreakChange.NoChange)
                    continue;

                yield return new TagSpan<BreakpointTag>(new SnapshotSpan(span.Start, span.Length), new BreakpointTag(extraData));
            }
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
    }
}
