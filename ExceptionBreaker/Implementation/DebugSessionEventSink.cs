using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;
using Microsoft.VisualStudio.Debugger.Interop.Internal;

namespace ExceptionBreaker.Implementation {
    public class DebugSessionEventSink : IDebuggerInternalEvents {
        public event EventHandler SessionCreated = delegate { };
        public event EventHandler SessionDestroyed = delegate { };

        public int OnSessionCreate(IDebugSession2 pSession) {
            this.SessionCreated(this, EventArgs.Empty);
            return VSConstants.S_OK;
        }

        public int OnSessionDestroy(IDebugSession2 pSession) {
            this.SessionDestroyed(this, EventArgs.Empty);
            return VSConstants.S_OK;
        }

        #region IDebuggerInternalEvents Members

        int IDebuggerInternalEvents.OnProcessRegister(IDebugCoreServer2 pServer, IDebugProcess2 pProcess) {
            return VSConstants.S_OK;
        }

        int IDebuggerInternalEvents.OnProcessDeregister(IDebugCoreServer2 pServer, IDebugProcess2 pProcess) {
            return VSConstants.S_OK;
        }

        int IDebuggerInternalEvents.OnCurrentProcessChange() {
            return VSConstants.S_OK;
        }

        int IDebuggerInternalEvents.OnCurrentProgramChange() {
            return VSConstants.S_OK;
        }

        int IDebuggerInternalEvents.OnCurrentThreadChange() {
            return VSConstants.S_OK;
        }

        int IDebuggerInternalEvents.OnCurrentFrameChange() {
            return VSConstants.S_OK;
        }

        int IDebuggerInternalEvents.OnCurrentStatementChange() {
            return VSConstants.S_OK;
        }

        int IDebuggerInternalEvents.OnModeChange(uint NewDebugMode) {
            return VSConstants.S_OK;
        }

        int IDebuggerInternalEvents.OnEnterRunMode() {
            return VSConstants.S_OK;
        }

        int IDebuggerInternalEvents.OnProcessCreate(IDebugProcess2 pProcess) {
            return VSConstants.S_OK;
        }

        int IDebuggerInternalEvents.OnProcessDestroy(IDebugProcess2 pProcess) {
            return VSConstants.S_OK;
        }

        int IDebuggerInternalEvents.OnThreadCreate(IDebugThread2 pThread) {
            return VSConstants.S_OK;
        }

        int IDebuggerInternalEvents.OnThreadDestroy(IDebugThread2 pThread) {
            return VSConstants.S_OK;
        }

        int IDebuggerInternalEvents.OnTimeContextChange() {
            return VSConstants.S_OK;
        }

        int IDebuggerInternalEvents.OnShellModeChange(uint NewShellMode) {
            return VSConstants.S_OK;
        }

        int IDebuggerInternalEvents.OnSetNextStatement() {
            return VSConstants.S_OK;
        }

        #endregion
    }
}