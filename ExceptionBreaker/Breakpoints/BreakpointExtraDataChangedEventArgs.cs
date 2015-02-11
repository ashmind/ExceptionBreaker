using System;
using EnvDTE80;
using JetBrains.Annotations;

namespace ExceptionBreaker.Breakpoints {
    public class BreakpointExtraDataChangedEventArgs : EventArgs {
        public BreakpointExtraDataChangedEventArgs(Breakpoint2 breakpoint, BreakpointExtraData data) {
            Breakpoint = breakpoint;
            Data = data;
        }

        [NotNull] public Breakpoint2 Breakpoint   { get; private set; }
        [NotNull] public BreakpointExtraData Data { get; private set; }
    }
}