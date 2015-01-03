using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using ExceptionBreaker.Core;
using Microsoft.VisualStudio.Text;

namespace ExceptionBreaker.Breakpoints {
    [Export]
    public class BreakpointFinder {
        private readonly DTE _dte;

        [ImportingConstructor]
        public BreakpointFinder(DTEImport dteImport) {
            _dte = dteImport.DTE;
        }

        public IEnumerable<Breakpoint2> GetAllBreakpoints() {
            return _dte.Debugger.Breakpoints.Cast<Breakpoint2>();
        }

        public Breakpoint2 GetBreakpointForCommand() {
            var path = _dte.ActiveDocument.FullName;
            var line = ((TextSelection)_dte.ActiveDocument.Selection).CurrentLine;
            return GetBreakpointFromLocation(path, line);
        }

        public Breakpoint2 GetBreakpointFromSpan(SnapshotSpan span, ITextDocument document) {
            var path = document.FilePath;
            var line = span.Snapshot.GetLineNumberFromPosition(span.Span.Start) + 1;
            return GetBreakpointFromLocation(path, line);
        }

        private Breakpoint2 GetBreakpointFromLocation(string path, int line) {
            return GetAllBreakpoints().FirstOrDefault(
                breakpoint => breakpoint.File == path && breakpoint.FileLine == line
            );
        }
    }
}
