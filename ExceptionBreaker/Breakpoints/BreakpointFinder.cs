using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using JetBrains.Annotations;
using Microsoft.VisualStudio.Text;

namespace ExceptionBreaker.Breakpoints {
    [Export]
    public class BreakpointFinder {
        private readonly DTE _dte;
        private readonly BreakpointObservableListProvider _breakpointListProvider;

        [ImportingConstructor]
        public BreakpointFinder(DTE dte, BreakpointObservableListProvider breakpointListProvider) {
            _dte = dte;
            _breakpointListProvider = breakpointListProvider;
        }

        public IEnumerable<Breakpoint2> GetAllBreakpoints() {
            return _breakpointListProvider.Breakpoints;
        }

        [CanBeNull]
        public Breakpoint2 GetBreakpointForCommand() {
            var path = _dte.ActiveDocument.FullName;
            var line = ((TextSelection)_dte.ActiveDocument.Selection).CurrentLine;
            return GetBreakpointFromLocation(path, line);
        }

        [CanBeNull]
        public Breakpoint2 GetBreakpointFromSpan(SnapshotSpan span, [NotNull] ITextDocument document) {
            var path = document.FilePath;
            var line = span.Snapshot.GetLineNumberFromPosition(span.Span.Start) + 1;
            return GetBreakpointFromLocation(path, line);
        }

        [CanBeNull]
        private Breakpoint2 GetBreakpointFromLocation(string path, int line) {
            return GetAllBreakpoints().FirstOrDefault(
                breakpoint => breakpoint.File == path && breakpoint.FileLine == line
            );
        }

        public SnapshotSpan GetSpanFromBreakpoint([NotNull] Breakpoint2 breakpoint, [NotNull] ITextDocument document) {
            var snapshot = document.TextBuffer.CurrentSnapshot;
            var line = snapshot.GetLineFromLineNumber(breakpoint.FileLine - 1);
            return line.Extent;
        }
    }
}
