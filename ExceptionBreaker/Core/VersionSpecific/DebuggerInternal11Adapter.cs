using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Debugger.Interop;
using Microsoft.VisualStudio.Debugger.Interop.Internal;

namespace ExceptionBreaker.Core.VersionSpecific {
    public class DebuggerInternal11Adapter : IDebuggerInternalAdapter {
        private readonly IDebuggerInternal11 _debugger;

        public DebuggerInternal11Adapter(IDebuggerInternal11 debugger) {
            _debugger = debugger;
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
