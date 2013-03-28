using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;

namespace ExceptionBreaker.Implementation {
    /// <summary>
    /// Manages current break state and applies state changes to <see cref="IDebugSession2" />.
    /// </summary>
    public class ExceptionBreakManager {
        // Note: I did not research any specific differences between JustMyCode=On/Off.
        // the constants below seem to work well enough, but I would not be surprised if
        // there is something I have not noticed

        private const enum_EXCEPTION_STATE VSExceptionStateStopAll =
            enum_EXCEPTION_STATE.EXCEPTION_STOP_FIRST_CHANCE
          | enum_EXCEPTION_STATE.EXCEPTION_STOP_SECOND_CHANCE
          | enum_EXCEPTION_STATE.EXCEPTION_JUST_MY_CODE_SUPPORTED
          | enum_EXCEPTION_STATE.EXCEPTION_STOP_USER_FIRST_CHANCE
          | enum_EXCEPTION_STATE.EXCEPTION_STOP_USER_UNCAUGHT;

        private const enum_EXCEPTION_STATE VSExceptionStateStopAllInfer =
            enum_EXCEPTION_STATE.EXCEPTION_STOP_FIRST_CHANCE
          | enum_EXCEPTION_STATE.EXCEPTION_STOP_SECOND_CHANCE
          | enum_EXCEPTION_STATE.EXCEPTION_STOP_USER_FIRST_CHANCE;

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

        // Just for convenience
        private IDebugSession2 Session {
            get { return this.watcher.DebugSession; }
        }

        public ExceptionBreakState CurrentState {
            get { return this.currentState; }
            set {
                if (this.currentState == value)
                    return;

                this.logger.WriteLine("Manager: CurrentState is being set to {0}.", value);

                var session = this.watcher.DebugSession;
                if (session != null)
                    this.EnsureManagedExceptionCache();

                if (value == ExceptionBreakState.BreakOnAll || value == ExceptionBreakState.BreakOnNone) {
                    if (this.exceptionCache == null) {
                        this.logger.WriteLine("Manager: Cache not available, cannot apply state to session.");
                        return;
                    }

                    this.ApplyStateToSession(value);
                }

                this.currentState = value;
                this.logger.WriteLine("Manager: CurrentState was set to {0}.", value);
                this.CurrentStateChanged(this, EventArgs.Empty);
            }
        }

        public void RefreshCurrentState() {
            // not using property here as it would try to re-apply state
            this.currentState = this.GetStateFromSession();
            this.CurrentStateChanged(this, EventArgs.Empty);
        }

        private void watcher_DebugSessionChanged(object sender, EventArgs e) {
            if (this.Session == null)
                return;

            if (this.currentState == ExceptionBreakState.Inconclusive || this.currentState == ExceptionBreakState.Unknown)
                RefreshCurrentState();
        }

        private void EnsureManagedExceptionCache() {
            if (this.exceptionCache != null)
                return;

            var guid = VSConstants.DebugEnginesGuids.ManagedOnly_guid;

            var root = this.GetDefaultExceptions();
            var list = new List<EXCEPTION_INFO>(root.Where(e => e.guidType == guid));
            var index = 0;

            while (index < list.Count) {
                var children = this.GetDefaultExceptions(list[index]);
                list.AddRange(children);

                index += 1;
            }

            this.exceptionCache = list;
            this.logger.WriteLine("Exception cache is built: {0} exceptions.", this.exceptionCache.Count);
        }

        /// <summary>
        /// Gets full list of child exceptions under given <paramref name="parent" />.
        /// The exceptions will not have a specific state set.
        /// </summary>
        private EXCEPTION_INFO[] GetDefaultExceptions(EXCEPTION_INFO? parent = null) {
            IEnumDebugExceptionInfo2 enumerator;
            var hr = this.Session.EnumDefaultExceptions(parent != null ? new[] { parent.Value } : null, out enumerator);

            return GetExceptionsFromEnumerator(hr, enumerator);
        }

        /// <summary>
        /// Gets list of all managed exceptions that were *set*. It is hard to understand what exactly 
        /// *set* means in this context, as it returns non-set exception in certain cases. 
        /// </summary>
        private EXCEPTION_INFO[] GetSetManagedExceptions() {
            var guid = VSConstants.DebugEnginesGuids.ManagedOnly_guid;

            IEnumDebugExceptionInfo2 enumerator;
            var hr = this.Session.EnumSetExceptions(null, null, guid, out enumerator);

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

        private ExceptionBreakState GetStateFromSession() {
            var inferredState = ExceptionBreakState.Unknown;
            var exceptionThatCausedChangeFromUnknown = new EXCEPTION_INFO();

            foreach (var exception in GetSetManagedExceptions()) {
                var @break = (((enum_EXCEPTION_STATE)exception.dwState & VSExceptionStateStopAllInfer) == VSExceptionStateStopAllInfer);
                var stateFromException = @break ? ExceptionBreakState.BreakOnAll : ExceptionBreakState.BreakOnNone;

                if (inferredState == ExceptionBreakState.Unknown) {
                    inferredState = stateFromException;
                    exceptionThatCausedChangeFromUnknown = exception;
                    continue;
                }

                if (inferredState != stateFromException) {
                    this.logger.WriteLine("Manager: inconclusive state diagnostic.");
                    this.logger.WriteLine("  Previous state:        {0}.", inferredState);
                    this.logger.WriteLine("  Previous exception:    {0}; {1}.", exceptionThatCausedChangeFromUnknown.bstrExceptionName, (enum_EXCEPTION_STATE)exceptionThatCausedChangeFromUnknown.dwState);
                    this.logger.WriteLine("  Conflicting state:     {0}.", stateFromException);
                    this.logger.WriteLine("  Conflicting exception: {0}; {1}.", exception.bstrExceptionName, (enum_EXCEPTION_STATE)exception.dwState);

                    inferredState = ExceptionBreakState.Inconclusive;
                    break;
                }
            }

            this.logger.WriteLine("Manager: inferred state is {0}.", inferredState);
            return inferredState;
        }

        private void ApplyStateToSession(ExceptionBreakState state) {
            this.logger.WriteLine("Manager: Applying {0} to debug session.", state);
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var newExceptionState = (state == ExceptionBreakState.BreakOnAll)
                                  ? (uint)VSExceptionStateStopAll
                                  : (uint)VSExceptionStateStopNotSet;

            var guid = Guid.Empty;
            var hr = this.Session.RemoveAllSetExceptions(ref guid);
            if (hr != VSConstants.S_OK)
                Marshal.ThrowExceptionForHR(hr);

            var updated = new EXCEPTION_INFO[1];
            foreach (var exception in this.exceptionCache) {
                updated[0] = new EXCEPTION_INFO {
                    guidType = exception.guidType,
                    bstrExceptionName = exception.bstrExceptionName,
                    dwState = newExceptionState
                };

                hr = this.Session.SetException(updated);
                if (hr != VSConstants.S_OK)
                    Marshal.ThrowExceptionForHR(hr);
            }

            stopwatch.Stop();
            this.logger.WriteLine("  Finished in {0}ms.", stopwatch.ElapsedMilliseconds);
        }
    }
}
