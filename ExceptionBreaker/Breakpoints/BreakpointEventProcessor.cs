using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using ExceptionBreaker.Core;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;
using Microsoft.VisualStudio.Shell.Interop;

namespace ExceptionBreaker.Breakpoints {
    public class BreakpointEventProcessor : IDebugEventCallback2, IDisposable {
        private readonly IVsDebugger _debugger;
        private readonly ExceptionBreakChangeStore _store;
        private readonly ExceptionBreakManager _breakManager;
        private readonly IDiagnosticLogger _logger;

        public BreakpointEventProcessor(IVsDebugger debugger, ExceptionBreakChangeStore store, ExceptionBreakManager breakManager, IDiagnosticLogger logger) {
            _debugger = debugger;
            _store = store;
            _breakManager = breakManager;
            _logger = logger;

            debugger.AdviseDebugEventCallback(this);
        }

        public int Event(IDebugEngine2 pEngine, IDebugProcess2 pProcess, IDebugProgram2 pProgram, IDebugThread2 pThread, IDebugEvent2 pEvent, ref Guid riidEvent, uint dwAttrib) {
            try {
                ProcessEvent(pEvent);
            }
            catch (Exception ex) {
                _logger.WriteLine("Unexpected exception: " + ex);
            }
            finally {
                VSInteropHelper.Release(pEngine);
                VSInteropHelper.Release(pProcess);
                VSInteropHelper.Release(pProgram);
                VSInteropHelper.Release(pThread);
                VSInteropHelper.Release(pEvent);
            }
            return VSConstants.S_OK;
        }

        private void ProcessEvent(IDebugEvent2 pEvent) {
            var breakpointEvent = pEvent as IDebugBreakpointEvent2;
            if (breakpointEvent == null)
                return;

            _logger.WriteLine("Event: Breakpoint reached.");
            foreach (var breakpoint in breakpointEvent.GetBreakpointsAsArraySafe()) {
                var change = _store.GetChange(breakpoint);
                _logger.WriteLine("Change is.... {0}", change);
            }
        }

        public void Dispose() {
            _debugger.UnadviseDebugEventCallback(this);
        }
    }
}
