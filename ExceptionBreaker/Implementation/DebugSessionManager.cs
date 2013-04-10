using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;
using Microsoft.VisualStudio.Debugger.Interop.Internal;

namespace ExceptionBreaker.Implementation {
    /// <summary>
    /// Monitors debug session create/destroy events and maintains session between them.
    /// </summary>
    public class DebugSessionManager : IDisposable {
        public event EventHandler DebugSessionChanged = delegate {};

        private readonly IDebuggerInternal debugger;
        private readonly IDiagnosticLogger logger;
        private readonly DebugSessionEventSink eventSink;

        public DebugSessionManager(IDebuggerInternal debugger, IDiagnosticLogger logger) {
            this.debugger = debugger;
            this.logger = logger;

            this.eventSink = new DebugSessionEventSink();
            this.eventSink.SessionCreated += (sender, args) => this.ProcessSessionCreateOrDestoryEvent("created");
            this.eventSink.SessionDestroyed += (sender, args) => this.ProcessSessionCreateOrDestoryEvent("destroyed");

            var hr = this.debugger.RegisterInternalEventSink(this.eventSink);
            if (hr != VSConstants.S_OK)
                Marshal.ThrowExceptionForHR(hr);
        }

        public IDebugSession2 DebugSession {
            get { return this.debugger.CurrentSession; }
        }

        private void ProcessSessionCreateOrDestoryEvent(string logSessionAs) {
            this.logger.WriteLine("Event: Debug session {0}.", logSessionAs);
            this.DebugSessionChanged(this, EventArgs.Empty);
        }

        public void Dispose() {
            this.debugger.UnregisterInternalEventSink(this.eventSink);
        }
    }
}