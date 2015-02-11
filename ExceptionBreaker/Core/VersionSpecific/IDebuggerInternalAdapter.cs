using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Debugger.Interop;
using Microsoft.VisualStudio.Debugger.Interop.Internal;

namespace ExceptionBreaker.Core.VersionSpecific {
    public interface IDebuggerInternalAdapter {
        IBreakpointManager BreakpointManager { get; }
        IDebugSession3 CurrentSession { get; }
        int RegisterInternalEventSink(DebugSessionEventSink eventSink);
        int UnregisterInternalEventSink(DebugSessionEventSink eventSink);
    }
}
