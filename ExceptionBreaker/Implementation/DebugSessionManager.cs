using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using ExceptionBreaker.Implementation.VersionSpecific;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;

namespace ExceptionBreaker.Implementation {
    /// <summary>
    /// Monitors debug session create/destroy events and maintains session between them.
    /// </summary>
    public class DebugSessionManager : IDisposable {
        public event EventHandler DebugSessionChanged = delegate {};

        private readonly IDebuggerInternalAdapter _debugger;
        private readonly IDiagnosticLogger _logger;
        private readonly DebugSessionEventSink _eventSink;

        public DebugSessionManager(IDebuggerInternalAdapter debugger, IDiagnosticLogger logger) {
            _debugger = debugger;
            _logger = logger;

            _eventSink = new DebugSessionEventSink();
            _eventSink.SessionCreated += (sender, args) => ProcessSessionCreateOrDestoryEvent("created");
            _eventSink.SessionDestroyed += (sender, args) => ProcessSessionCreateOrDestoryEvent("destroyed");

            var hr = _debugger.RegisterInternalEventSink(_eventSink);
            if (hr != VSConstants.S_OK)
                Marshal.ThrowExceptionForHR(hr);
        }

        public IDebugSession2 DebugSession {
            get { return _debugger.CurrentSession; }
        }

        private void ProcessSessionCreateOrDestoryEvent(string logSessionAs) {
            _logger.WriteLine("Event: Debug session {0}.", logSessionAs);
            DebugSessionChanged(this, EventArgs.Empty);
        }

        public void Dispose() {
            _debugger.UnregisterInternalEventSink(_eventSink);
        }
    }
}