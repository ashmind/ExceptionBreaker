using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Debugger.Interop;
using Microsoft.VisualStudio.Debugger.Interop.Internal;

namespace ExceptionBreaker.Core.VersionSpecific {
    public class DebuggerInternal10Adapter : IDebuggerInternalAdapter {
        private readonly IDebuggerInternal10 _debugger;

        public DebuggerInternal10Adapter(IDebuggerInternal10 debugger) {
            _debugger = debugger;
        }

        public IBreakpointManager BreakpointManager {
            get { return _debugger.BreakpointManager; }
        }

        public IDebugSession3 CurrentSession {
            get { return _debugger.CurrentSession; }
        }

        public int RegisterInternalEventSink(DebugSessionEventSink pEvents) {
            return _debugger.RegisterInternalEventSink(pEvents);
        }

        public int UnregisterInternalEventSink(DebugSessionEventSink pEvents) {
            return _debugger.UnregisterInternalEventSink(pEvents);
        }
    }
}
