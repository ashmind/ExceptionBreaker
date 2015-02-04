using System;
using EnvDTE80;

namespace ExceptionBreaker.Breakpoints {
    public class BreakpointExtraDataChangedEventArgs : EventArgs {
        public BreakpointExtraDataChangedEventArgs(Breakpoint2 breakpoint) {
            Breakpoint = breakpoint;
        }

        public Breakpoint2 Breakpoint { get; private set; }
    }
}