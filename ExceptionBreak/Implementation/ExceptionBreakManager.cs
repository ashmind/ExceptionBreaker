using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;

namespace ExceptionBreak.Implementation {
    public class ExceptionBreakManager {
        private const enum_EXCEPTION_STATE VSExceptionStateStopAll =
            enum_EXCEPTION_STATE.EXCEPTION_STOP_FIRST_CHANCE
          | enum_EXCEPTION_STATE.EXCEPTION_STOP_SECOND_CHANCE
          | enum_EXCEPTION_STATE.EXCEPTION_JUST_MY_CODE_SUPPORTED
          | enum_EXCEPTION_STATE.EXCEPTION_STOP_USER_FIRST_CHANCE
          | enum_EXCEPTION_STATE.EXCEPTION_STOP_USER_UNCAUGHT;

        // I honestly do not understand this one yet.
        // However it is equivalent to what Debug->Exceptions window does.
        private const enum_EXCEPTION_STATE VSExceptionStateStopNotSet =
            enum_EXCEPTION_STATE.EXCEPTION_STOP_SECOND_CHANCE
          | enum_EXCEPTION_STATE.EXCEPTION_JUST_MY_CODE_SUPPORTED;

        public event EventHandler CurrentStateChanged = delegate { };

        private readonly DebugSessionWatcher watcher;
        private readonly IDiagnosticLogger logger;

        private ICollection<EXCEPTION_INFO> exceptionCache;
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

                this.logger.WriteLine("Manager: CurrentState is being set to {0}.", value);

                var session = this.watcher.DebugSession;
                if (session != null)
                    this.EnsureManagedExceptionCache(session);

                if (value == ExceptionBreakState.BreakOnAll || value == ExceptionBreakState.BreakOnNone) {
                    if (this.exceptionCache != null) {
                        this.ApplyStateToSession(value, session);
                    }
                    else {
                        this.logger.WriteLine("Manager: Cache not available, cannot apply state to session.");
                        return;
                    }
                }

                this.currentState = value;
                this.logger.WriteLine("Manager: CurrentState was set to {0}.", value);
                this.CurrentStateChanged(this, EventArgs.Empty);
            }
        }

        private void watcher_DebugSessionChanged(object sender, EventArgs e) {
            var session = this.watcher.DebugSession;
            if (session == null)
                return;

            this.EnsureManagedExceptionCache(session);
            if (this.currentState == ExceptionBreakState.Inconclusive || this.currentState == ExceptionBreakState.Unknown) {
                this.currentState = this.GetStateFromSession(session);
                this.CurrentStateChanged(this, EventArgs.Empty);
            }
        }

        private void EnsureManagedExceptionCache(IDebugSession2 debugSession) {
            if (this.exceptionCache != null)
                return;

            var guid = VSConstants.DebugEnginesGuids.ManagedOnly_guid;

            var root = this.GetDefaultExceptions(debugSession);
            var list = new List<EXCEPTION_INFO>(root.Where(e => e.guidType == guid));
            var index = 0;

            while (index < list.Count) {
                var children = this.GetDefaultExceptions(debugSession, list[index]);
                list.AddRange(children);

                index += 1;
            }

            this.exceptionCache = list;
            this.logger.WriteLine("Exception cache is built: {0} exceptions.", this.exceptionCache.Count);
        }

        private EXCEPTION_INFO[] GetDefaultExceptions(IDebugSession2 debugSession, EXCEPTION_INFO? parent = null) {
            IEnumDebugExceptionInfo2 enumerator;
            var hr = debugSession.EnumDefaultExceptions(parent != null ? new[] { parent.Value } : null, out enumerator);

            return GetExceptionsFromEnumerator(hr, enumerator);
        }

        private EXCEPTION_INFO[] GetSetManagedExceptions(IDebugSession2 debugSession) {
            var guid = VSConstants.DebugEnginesGuids.ManagedOnly_guid;

            IEnumDebugExceptionInfo2 enumerator;
            var hr = debugSession.EnumSetExceptions(null, null, guid, out enumerator);

            return GetExceptionsFromEnumerator(hr, enumerator);
        }

        private static EXCEPTION_INFO[] GetExceptionsFromEnumerator(int enumHResult, IEnumDebugExceptionInfo2 enumerator) {
            if (enumHResult == VSConstants.S_FALSE)
                return new EXCEPTION_INFO[0];

            if (enumHResult != VSConstants.S_OK)
                Marshal.ThrowExceptionForHR(enumHResult);

            uint count;
            var hr = enumerator.GetCount(out count);
            if (hr != VSConstants.S_OK)
                Marshal.ThrowExceptionForHR(hr);

            if (count == 0)
                return new EXCEPTION_INFO[0];

            var buffer = new EXCEPTION_INFO[count];
            var countFetched = 0U;
            hr = enumerator.Next(count, buffer, ref countFetched);
            if (hr != VSConstants.S_OK)
                Marshal.ThrowExceptionForHR(hr);

            return buffer;
        }

        private ExceptionBreakState GetStateFromSession(IDebugSession2 session) {
            var inferredState = ExceptionBreakState.Unknown;
            foreach (var exception in GetSetManagedExceptions(session)) {
                var @break = (((enum_EXCEPTION_STATE)exception.dwState & VSExceptionStateStopAll) == VSExceptionStateStopAll);
                var stateFromException = @break ? ExceptionBreakState.BreakOnAll : ExceptionBreakState.BreakOnNone;

                if (inferredState == ExceptionBreakState.Unknown) {
                    inferredState = stateFromException;
                    continue;
                }

                if (inferredState != stateFromException) {
                    var exceptionStateAsEnum = (enum_EXCEPTION_STATE)exception.dwState;
                    this.logger.WriteLine("Manager, inconclusive state diagnostic:");
                    this.logger.WriteLine("  Expected state:      {0}.", inferredState);
                    this.logger.WriteLine("  Exception details:   {0}; {1}.", exception.bstrExceptionName, exceptionStateAsEnum);
                    this.logger.WriteLine("  Actual state:        {0}.", stateFromException);

                    inferredState = ExceptionBreakState.Inconclusive;
                    break;
                }
            }

            this.logger.WriteLine("Manager, inferred state: {0}.", inferredState);
            return inferredState;
        }

        private void ApplyStateToSession(ExceptionBreakState state, IDebugSession2 session) {
            this.logger.WriteLine("Manager: Applying {0} to debug session.", state);
            var newExceptionState = (state == ExceptionBreakState.BreakOnAll)
                                  ? (uint)VSExceptionStateStopAll
                                  : (uint)VSExceptionStateStopNotSet;

            var guid = Guid.Empty;
            var hr = session.RemoveAllSetExceptions(ref guid);
            if (hr != VSConstants.S_OK)
                Marshal.ThrowExceptionForHR(hr);

            foreach (var exception in this.exceptionCache) {
                SetException(session, exception, newExceptionState);
            }
        }

        private void SetException(IDebugSession2 session, EXCEPTION_INFO @default, uint dwNewState, IDebugProgram2 program = null) {
            var updated = new EXCEPTION_INFO {
                guidType = @default.guidType,
                bstrExceptionName = @default.bstrExceptionName,
                dwState = dwNewState,
                pProgram = program
            };
            
            var hr = session.SetException(new[] { updated });
            if (hr != VSConstants.S_OK)
                Marshal.ThrowExceptionForHR(hr);
        }

        //private static EXCEPTION_INFO DEBUG_GetException(IDebugSession2 session, EXCEPTION_INFO exception, IDebugProgram2 program = null) {
        //    var guid = VSConstants.DebugEnginesGuids.ManagedOnly_guid;
        //    IEnumDebugExceptionInfo2 enumerator;
        //    var hr = session.EnumSetExceptions(program, null, ref guid, out enumerator);
        //    if (hr != VSConstants.S_OK)
        //        Marshal.ThrowExceptionForHR(hr);

        //    uint count;
        //    hr = enumerator.GetCount(out count);
        //    if (hr != VSConstants.S_OK)
        //        Marshal.ThrowExceptionForHR(hr);

        //    var buffer = new EXCEPTION_INFO[count];
        //    var countFetched = 0U;
        //    hr = enumerator.Next(count, buffer, ref countFetched);
        //    if (hr != VSConstants.S_OK)
        //        Marshal.ThrowExceptionForHR(hr);

        //    return buffer.FirstOrDefault(e => e.bstrExceptionName == exception.bstrExceptionName);
        //}

        //private static string DEBUG_DescribeExceptionState(uint dwState) {
        //    return ((enum_EXCEPTION_STATE)dwState).ToString();
        //}
    }
}
