using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;

namespace ExceptionBreaker.Breakpoints {
    public class BreakpointFinder {
        private readonly DTE _dte;

        public BreakpointFinder(DTE dte) {
            _dte = dte;
        }

        public Breakpoint2 GetBreakpointForCommand() {
            var path = _dte.ActiveDocument.FullName;
            var line = ((TextSelection)_dte.ActiveDocument.Selection).CurrentLine;

            var breakpoints = _dte.Debugger.Breakpoints;
            for (var i = 1; i <= breakpoints.Count; i++) {
                var breakpoint = breakpoints.Item(i);
                if (breakpoint.File == path && breakpoint.FileLine == line)
                    return (Breakpoint2)breakpoint;
            }

            return null;
        }
    }
}
