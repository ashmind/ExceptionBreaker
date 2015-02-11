using System;
using System.Collections.Generic;
using System.Linq;
using ExceptionBreaker.Core;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;
using Microsoft.VisualStudio.Shell.Interop;

namespace ExceptionBreaker.Breakpoints {
    public class BreakpointHitHandler : IDebugEventCallback2, IDisposable {
        private readonly IVsDebugger _debugger;
        private readonly BreakpointExtraDataStore _extraDataStore;
        private readonly ExceptionBreakManager _breakManager;
        private readonly IDiagnosticLogger _logger;
        private readonly ISet<BreakpointExtraData> _actionableExtraData = new HashSet<BreakpointExtraData>();
        private bool _eventsAdvised;

        public BreakpointHitHandler(IVsDebugger debugger, BreakpointExtraDataStore extraDataStore, ExceptionBreakManager breakManager, IDiagnosticLogger logger) {
            _debugger = debugger;
            _extraDataStore = extraDataStore;
            _breakManager = breakManager;
            _logger = logger;

            // Optimization: I expect that most ppl would not use breakpoints feature,
            // so here we ignore events unless there are actual breakpoints
            ClassifyAllExtraData();
            UpdateEventSubscription();

            _extraDataStore.DataLoaded += (sender, e) => {
                ClassifyAllExtraData();
                UpdateEventSubscription();
            };
            _extraDataStore.DataChanged += (sender, e) => {
                if (e.Data.ExceptionBreakChange != ExceptionBreakChange.NoChange) {
                    _actionableExtraData.Add(e.Data);
                }
                else {
                    _actionableExtraData.Remove(e.Data);
                }
                UpdateEventSubscription();
            };
        }

        private void ClassifyAllExtraData() {
            _actionableExtraData.Clear();
            foreach (var data in _extraDataStore.GetAllCurrentData()) {
                if (data.ExceptionBreakChange != ExceptionBreakChange.NoChange)
                    _actionableExtraData.Add(data);
            }
        }

        private void UpdateEventSubscription() {
            if (_actionableExtraData.Count > 0) {
                if (!_eventsAdvised) {
                    _logger.WriteLine("Found actionable breakpoints, activating breakpoint events.");
                    _debugger.AdviseDebugEventCallback(this);
                    _eventsAdvised = true;
                }
            }
            else {
                if (_eventsAdvised) {
                    _logger.WriteLine("No actionable breakpoints, deactivating breakpoint events.");
                    _debugger.UnadviseDebugEventCallback(this);
                    _eventsAdvised = false;
                }
            }
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

            foreach (var breakpoint in breakpointEvent.GetBreakpointsAsArraySafe()) {
                var extraData = _extraDataStore.GetData(breakpoint);
                if (extraData == null)
                    continue;

                var change = extraData.ExceptionBreakChange;
                if (change == ExceptionBreakChange.NoChange)
                    continue;

                _logger.WriteLine("Breakpoint requires exception state change: {0}.", change);
                _breakManager.CurrentState = change == ExceptionBreakChange.SetBreakOnAll
                                           ? ExceptionBreakState.BreakOnAll
                                           : ExceptionBreakState.BreakOnNone;
            }
        }

        public void Dispose() {
            if (_eventsAdvised)
                _debugger.UnadviseDebugEventCallback(this);
        }
    }
}
