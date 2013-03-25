using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;
using Microsoft.VisualStudio.Shell.Interop;

namespace ExceptionBreak.Implementation {
    public class DebugSessionWatcher : IDebugEventCallback2, IDisposable {
        public event EventHandler DebugSessionChanged = delegate {};

        public IDebugSession2 DebugSession { get; set; }

        private readonly IVsDebugger debugger;
        private readonly IDiagnosticLogger logger;

        public DebugSessionWatcher(IVsDebugger debugger, IDiagnosticLogger logger) {
            this.debugger = debugger;
            this.logger = logger;

            var hr = this.debugger.AdviseDebugEventCallback(this);
            if (hr != VSConstants.S_OK)
                Marshal.ThrowExceptionForHR(hr);
        }

        int IDebugEventCallback2.Event(IDebugEngine2 pEngine, IDebugProcess2 pProcess, IDebugProgram2 pProgram, IDebugThread2 pThread, IDebugEvent2 pEvent, ref Guid riidEvent, uint dwAttrib) {
            //this.logger.WriteLine("Debug event: pEngine = {0}, pProcess = {1}, pProgram = {2}, pThread = {3}, pEvent = {4}, riidEvent = {5}, dwAttrib = {6}",
            //                        pEngine, pProcess, pProgram, pThread, pEvent, riidEvent, dwAttrib);

            if (riidEvent != typeof(IDebugSessionCreateEvent2).GUID)
                return VSConstants.S_OK;

            this.logger.WriteLine("DebugSessionCreateEvent2");

            try {
                var sessionEvent = (IDebugSessionEvent2)pEvent;
                IDebugSession2 session;
                var hr = sessionEvent.GetSession(out session);
                if (hr != VSConstants.S_OK)
                    Marshal.ThrowExceptionForHR(hr);

                this.DebugSession = session;
                this.DebugSessionChanged(this, EventArgs.Empty);
            }
            catch (Exception ex) {
                this.logger.WriteLine("Unexpected exception: " + ex);
            }

            return VSConstants.S_OK;
        }

        public void Dispose() {
            this.debugger.UnadviseDebugEventCallback(this);
        }

        //int IDebuggerInternalEvents.OnSessionCreate(IDebugSession2 pSession) {
        //    try {
        //        this.logger.WriteLine("IDebuggerInternalEvents.OnSessionCreate");
        //        this.DebugSessionChanged(this, EventArgs.Empty);
        //    }
        //    catch (Exception ex) {
        //        this.logger.WriteLine("Unexpected exception: " + ex);
        //    }
        //    return VSConstants.S_OK;
        //}

        //int IDebuggerInternalEvents.OnSessionDestroy(IDebugSession2 pSession) {
        //    try {
        //        this.logger.WriteLine("IDebuggerInternalEvents.OnSessionDestroy");
        //        this.DebugSessionChanged(this, EventArgs.Empty);
        //    }
        //    catch (Exception ex) {
        //        this.logger.WriteLine("Unexpected exception: " + ex);
        //    }
        //    return VSConstants.S_OK;
        //}

        //#region IDebuggerInternalEvents Members

        //int IDebuggerInternalEvents.OnCurrentFrameChange() {
        //    return VSConstants.S_OK;
        //}

        //int IDebuggerInternalEvents.OnCurrentProcessChange() {
        //    return VSConstants.S_OK;
        //}

        //int IDebuggerInternalEvents.OnCurrentProgramChange() {
        //    return VSConstants.S_OK;
        //}

        //int IDebuggerInternalEvents.OnCurrentStatementChange() {
        //    return VSConstants.S_OK;
        //}

        //int IDebuggerInternalEvents.OnCurrentThreadChange() {
        //    return VSConstants.S_OK;
        //}

        //int IDebuggerInternalEvents.OnEnterRunMode() {
        //    return VSConstants.S_OK;
        //}

        //int IDebuggerInternalEvents.OnModeChange(uint NewDebugMode) {
        //    return VSConstants.S_OK;
        //}

        //int IDebuggerInternalEvents.OnProcessCreate(IDebugProcess2 pProcess) {
        //    return VSConstants.S_OK;
        //}

        //int IDebuggerInternalEvents.OnProcessDeregister(IDebugCoreServer2 pServer, IDebugProcess2 pProcess) {
        //    return VSConstants.S_OK;
        //}

        //int IDebuggerInternalEvents.OnProcessDestroy(IDebugProcess2 pProcess) {
        //    return VSConstants.S_OK;
        //}

        //int IDebuggerInternalEvents.OnProcessRegister(IDebugCoreServer2 pServer, IDebugProcess2 pProcess) {
        //    return VSConstants.S_OK;
        //}

        //int IDebuggerInternalEvents.OnSetNextStatement() {
        //    return VSConstants.S_OK;
        //}

        //int IDebuggerInternalEvents.OnShellModeChange(uint NewShellMode) {
        //    return VSConstants.S_OK;
        //}

        //int IDebuggerInternalEvents.OnThreadCreate(IDebugThread2 pThread) {
        //    return VSConstants.S_OK;
        //}

        //int IDebuggerInternalEvents.OnThreadDestroy(IDebugThread2 pThread) {
        //    return VSConstants.S_OK;
        //}

        //int IDebuggerInternalEvents.OnTimeContextChange() {
        //    return VSConstants.S_OK;
        //}

        //#endregion
    }
}