using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Debugger.Interop;

namespace ExceptionBreaker.Implementation.VersionSpecific {
    public interface IDebuggerInternalAdapter {
        IDebugSession3 CurrentSession { get; }
        int RegisterInternalEventSink(DebugSessionEventSink eventSink);
        int UnregisterInternalEventSink(DebugSessionEventSink eventSink);
    }
}
