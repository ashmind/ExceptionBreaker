using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;
using Microsoft.VisualStudio.Shell.Interop;

namespace ExceptionBreaker.Implementation {
    /// <summary>
    /// Monitors debug session create/destroy events and maintains session between them.
    /// </summary>
    public class DebugSessionWatcher : IDebugEventCallback2, IDisposable {
        public event EventHandler DebugSessionChanged = delegate {};

        private readonly IVsDebugger debugger;
        private readonly IDiagnosticLogger logger;

        private IDebugSession2 debugSession;

        public DebugSessionWatcher(IVsDebugger debugger, IDiagnosticLogger logger) {
            this.debugger = debugger;
            this.logger = logger;

            var hr = this.debugger.AdviseDebugEventCallback(this);
            if (hr != VSConstants.S_OK)
                Marshal.ThrowExceptionForHR(hr);
        }

        // accessing this during DebugSessionChanged might or might not be a race condition
        // (depending on how many threads are there, sending IDebugSessionCreateEvent2 events)
        // I am not looking into this question for now: you are welcome to improve it if needed
        public IDebugSession2 DebugSession {
            get { return this.debugSession; }
            private set {
                if (value == this.debugSession)
                    return;
                
                this.logger.WriteLine("DebugSession: Changing to {0}.", this.ToComPtrString(value));
                this.debugSession = value;
                this.DebugSessionChanged(this, EventArgs.Empty);
                this.logger.WriteLine("DebugSession: Changed to {0}.", this.ToComPtrString(value));
            }
        }

        private int ProcessSessionCreateOrAttachEvent(IDebugSessionEvent2 @event, string logSessionAs) {
            IDebugSession2 session;
            this.logger.WriteLine("Event: Debug session {0}.", logSessionAs);

            var hr = @event.GetSession(out session);
            if (hr != VSConstants.S_OK)
                Marshal.ThrowExceptionForHR(hr);
            
            this.DebugSession = session;
            return VSConstants.S_OK;
        }

        private int ProcessSessionDestroyEvent() {
            this.logger.WriteLine("Event: Debug session destroyed.");
            this.DebugSession = null;
            return VSConstants.S_OK;
        }

        int IDebugEventCallback2.Event(IDebugEngine2 pEngine, IDebugProcess2 pProcess, IDebugProgram2 pProgram, IDebugThread2 pThread, IDebugEvent2 pEvent, ref Guid riidEvent, uint dwAttrib) {
            try {
                if (riidEvent == typeof(IDebugSessionCreateEvent2).GUID)
                    return this.ProcessSessionCreateOrAttachEvent((IDebugSessionEvent2)pEvent, "created");

                if (riidEvent == typeof(IDebugAttachCompleteEvent2).GUID)
                    return this.ProcessSessionCreateOrAttachEvent((IDebugSessionEvent2)pEvent, "attached");

                if (riidEvent == typeof(IDebugSessionDestroyEvent2).GUID)
                    return this.ProcessSessionDestroyEvent();
            }
            catch (Exception ex) {
                this.logger.WriteLine("Unexpected exception: " + ex);
            }

            return VSConstants.S_OK;
        }

        private string ToComPtrString(object comObject) {
            if (comObject == null)
                return "null";

            var punk = IntPtr.Zero;
            try {
                punk = Marshal.GetIUnknownForObject(comObject);
                return punk.ToString();
            }
            finally {
                if (punk != IntPtr.Zero)
                    Marshal.Release(punk);
            }
        }

        public void Dispose() {
            this.debugger.UnadviseDebugEventCallback(this);
        }
    }
}