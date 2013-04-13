using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Debugger.Interop;
using Microsoft.VisualStudio.Debugger.Interop.Internal;

namespace ExceptionBreaker.Implementation.VersionSpecific {
    public class DebuggerInternal10Adapter : IDebuggerInternalAdapter {
        private readonly IDebuggerInternal10 debugger;

        public DebuggerInternal10Adapter(IDebuggerInternal10 debugger) {
            this.debugger = debugger;
        }

        public IDebugSession3 CurrentSession {
            get { return this.debugger.CurrentSession; }
        }
        
        public int RegisterInternalEventSink(DebugSessionEventSink pEvents) {
            return this.debugger.RegisterInternalEventSink(pEvents);
        }

        public int UnregisterInternalEventSink(DebugSessionEventSink pEvents) {
            return this.debugger.UnregisterInternalEventSink(pEvents);
        }
    }
}
