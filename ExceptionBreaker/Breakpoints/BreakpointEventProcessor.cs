using System;
using System.Collections.Generic;
using System.Linq;
using ExceptionBreaker.Core;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;
using Microsoft.VisualStudio.Shell.Interop;

namespace ExceptionBreaker.Breakpoints {
    public class BreakpointEventProcessor : IDebugEventCallback2, IDisposable {
        private readonly IVsDebugger _debugger;
        private readonly BreakpointExtraDataStore _extraDataStore;
        private readonly ExceptionBreakManager _breakManager;
        private readonly IDiagnosticLogger _logger;

        public BreakpointEventProcessor(IVsDebugger debugger, BreakpointExtraDataStore extraDataStore, ExceptionBreakManager breakManager, IDiagnosticLogger logger) {
            _debugger = debugger;
            _extraDataStore = extraDataStore;
            _breakManager = breakManager;
            _logger = logger;

            debugger.AdviseDebugEventCallback(this);
        }

        public int Event(IDebugEngine2 pEngine, IDebugProcess2 pProcess, IDebugProgram2 pProgram, IDebugThread2 pThread, IDebugEvent2 pEvent, ref Guid riidEvent, uint dwAttrib) {
            try {
                ProcessEvent(pEvent);
            }
            catch (Exception ex) {
                _logger.WriteLine("Unexpected exception: {0}", ex);
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
                var extraData = _extraDataStore.GetData(breakpoint);
                if (extraData == null || extraData.ExceptionBreakChange == ExceptionBreakChange.NoChange)
                    continue;

                var change = extraData.ExceptionBreakChange;
                _logger.WriteLine("Breakpoint requires exception state change: {0}.", change);
                _breakManager.CurrentState = change == ExceptionBreakChange.SetBreakOnAll
                                           ? ExceptionBreakState.BreakOnAll
                                           : ExceptionBreakState.BreakOnNone;
            }
        }

        public void Dispose() {
            _debugger.UnadviseDebugEventCallback(this);
        }
    }
}
