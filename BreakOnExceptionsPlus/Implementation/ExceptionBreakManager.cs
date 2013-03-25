using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;

namespace BreakOnExceptionsPlus.Implementation {
    public class ExceptionBreakManager {
        private const enum_EXCEPTION_STATE VSExceptionStateStopAll =
            enum_EXCEPTION_STATE.EXCEPTION_STOP_FIRST_CHANCE
          | enum_EXCEPTION_STATE.EXCEPTION_STOP_SECOND_CHANCE
          | enum_EXCEPTION_STATE.EXCEPTION_JUST_MY_CODE_SUPPORTED
          | enum_EXCEPTION_STATE.EXCEPTION_STOP_USER_FIRST_CHANCE
          | enum_EXCEPTION_STATE.EXCEPTION_STOP_USER_UNCAUGHT;

        public event EventHandler CurrentStateChanged = delegate { };

        private readonly DebugSessionWatcher watcher;
        private readonly IDiagnosticLogger logger;

        private IList<EXCEPTION_INFO> exceptionCache;
        private ExceptionBreakState currentState;

        public ExceptionBreakManager(DebugSessionWatcher watcher, IDiagnosticLogger logger) {
            this.watcher = watcher;
            this.logger = logger;

            this.watcher.DebugSessionChanged += watcher_DebugSessionChanged;
        }

        public ExceptionBreakState CurrentState {
            get { return this.currentState; }
            set {
                if (this.currentState == value)
                    return;

                this.currentState = value;
                if (exceptionCache != null && (value == ExceptionBreakState.BreakOnAll || value == ExceptionBreakState.BreakOnNone))
                    this.ApplyCurrentState(this.watcher.DebugSession);

                this.CurrentStateChanged(this, EventArgs.Empty);
            }
        }

        private void watcher_DebugSessionChanged(object sender, EventArgs e) {
            var session = this.watcher.DebugSession;
            if (session == null) {
                exceptionCache = null;
                this.CurrentState = ExceptionBreakState.Unknown;
                return;
            }

            exceptionCache = this.GetManagedExceptions(session);
            var state = this.CurrentState;
            if (state == ExceptionBreakState.BreakOnAll || state == ExceptionBreakState.BreakOnNone) {
                this.ApplyCurrentState(session);
            }
            else {
                this.DetectCurrentState();
            }
        }

        private IList<EXCEPTION_INFO> GetManagedExceptions(IDebugSession2 debugSession) {
            var guid = VSConstants.DebugEnginesGuids.ManagedOnly_guid;
            IEnumDebugExceptionInfo2 enumerator;
            var hr = debugSession.EnumSetExceptions(null, null, ref guid, out enumerator);
            if (hr != VSConstants.S_OK)
                Marshal.ThrowExceptionForHR(hr);

            uint count;
            hr = enumerator.GetCount(out count);
            if (hr != VSConstants.S_OK)
                Marshal.ThrowExceptionForHR(hr);

            var buffer = new EXCEPTION_INFO[count];
            var countFetched = 0U;
            hr = enumerator.Next(count, buffer, ref countFetched);
            if (hr != VSConstants.S_OK)
                Marshal.ThrowExceptionForHR(hr);

            return buffer;
        }
        
        private void DetectCurrentState() {
            var inferredState = ExceptionBreakState.Unknown;
            foreach (var exception in this.exceptionCache) {
                var @break = ((exception.dwState & (uint)VSExceptionStateStopAll) == (uint)VSExceptionStateStopAll);
                var stateFromBreak = @break ? ExceptionBreakState.BreakOnAll : ExceptionBreakState.BreakOnNone;

                if (inferredState == ExceptionBreakState.Unknown) {
                    inferredState = stateFromBreak;
                    continue;
                }

                if (inferredState != stateFromBreak) {
                    var exceptionStateAsEnum = (enum_EXCEPTION_STATE)exception.dwState;
                    this.logger.WriteLine("Inconclusive state diagnostic:");
                    this.logger.WriteLine("  Exception details:   {0}; {1}.", exception.bstrExceptionName, exceptionStateAsEnum);
                    this.logger.WriteLine("  State so far:        {0}.", inferredState);
                    this.logger.WriteLine("  State for exception: {0}.", stateFromBreak);

                    inferredState = ExceptionBreakState.Inconclusive;
                    break;
                }
            }

            this.logger.WriteLine("CurrentState was inferred to be {0}.", inferredState);
            this.currentState = inferredState;
            this.CurrentStateChanged(this, EventArgs.Empty);
        }
        
        private void ApplyCurrentState(IDebugSession2 session) {
            this.logger.WriteLine("Applying CurrentState: {0}.", this.CurrentState);
            if (this.CurrentState == ExceptionBreakState.BreakOnAll) {
                foreach (var exception in this.exceptionCache) {
                    var updated = new EXCEPTION_INFO {
                        guidType = exception.guidType,
                        bstrExceptionName = exception.bstrExceptionName,
                        dwState = (uint)VSExceptionStateStopAll
                    };

                    var hr = session.SetException(new[] { updated });
                    if (hr != VSConstants.S_OK)
                        Marshal.ThrowExceptionForHR(hr);
                }
            }
            else {
                foreach (var exception in this.exceptionCache) {
                    var hr = session.RemoveSetException(new[] { exception });
                    if (hr != VSConstants.S_OK)
                        Marshal.ThrowExceptionForHR(hr);
                }
            }
        }
    }
}
