using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;

namespace ExceptionBreaker.Core {
    /// <summary>
    /// Manages current break state and applies state changes to <see cref="IDebugSession2" />.
    /// </summary>
    public class ExceptionBreakManager : IExceptionListProvider {
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
        public event EventHandler ExceptionNamesChanged = delegate { };

        private readonly DebugSessionManager _sessionManager;
        private readonly Func<string, bool> _ignorePredicate;
        private readonly IDiagnosticLogger _logger;

        private ICollection<EXCEPTION_INFO> _exceptionCache;
        private ExceptionBreakState _currentState;

        public ExceptionBreakManager(DebugSessionManager sessionManager, Func<string, bool> ignorePredicate, IDiagnosticLogger logger) {
            _sessionManager = sessionManager;
            _ignorePredicate = ignorePredicate;
            _logger = logger;

            if (Session != null)
                _currentState = GetStateFromSession();
            _sessionManager.DebugSessionChanged += sessionManager_DebugSessionChanged;
        }

        // Just for convenience
        private IDebugSession2 Session {
            get { return _sessionManager.DebugSession; }
        }

        public ExceptionBreakState CurrentState {
            get { return _currentState; }
            set {
                if (_currentState == value)
                    return;

                _logger.WriteLine("Manager: CurrentState is being set to {0}.", value);

                if (Session != null)
                    EnsureManagedExceptionCache();

                if (value == ExceptionBreakState.BreakOnAll || value == ExceptionBreakState.BreakOnNone) {
                    if (_exceptionCache == null) {
                        _logger.WriteLine("Manager: Cache not available, cannot apply state to session.");
                        return;
                    }

                    ApplyStateToSession(value);
                }

                _currentState = value;
                _logger.WriteLine("Manager: CurrentState was set to {0}.", value);
                CurrentStateChanged(this, EventArgs.Empty);
            }
        }

        public IEnumerable<string> GetExceptionNames() {
            if (Session == null)
                return Enumerable.Empty<string>();

            EnsureManagedExceptionCache();
            return _exceptionCache.Select(e => e.bstrExceptionName);
        }

        public void RefreshCurrentState() {
            // not using property here as it would try to re-apply state
            _currentState = GetStateFromSession();
            CurrentStateChanged(this, EventArgs.Empty);
        }

        private void sessionManager_DebugSessionChanged(object sender, EventArgs e) {
            if (Session == null)
                return;

            EnsureManagedExceptionCache();
            if (_currentState == ExceptionBreakState.Inconclusive || _currentState == ExceptionBreakState.Unknown)
                RefreshCurrentState();
        }

        private void EnsureManagedExceptionCache() {
            if (_exceptionCache != null)
                return;

            var guid = VSConstants.DebugEnginesGuids.ManagedOnly_guid;

            var root = GetDefaultExceptions();
            var list = new List<EXCEPTION_INFO>(root.Where(e => e.guidType == guid));
            var index = 0;

            while (index < list.Count) {
                var children = GetDefaultExceptions(list[index]);
                list.AddRange(children);

                index += 1;
            }

            _exceptionCache = list;
            _logger.WriteLine("Exception cache is built: {0} exceptions.", _exceptionCache.Count);
            ExceptionNamesChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Gets full list of child exceptions under given <paramref name="parent" />.
        /// The exceptions will not have a specific state set.
        /// </summary>
        private EXCEPTION_INFO[] GetDefaultExceptions(EXCEPTION_INFO? parent = null) {
            return Session.GetDefaultExceptionsAsArraySafe(parent != null ? new[] {parent.Value} : null);
        }

        /// <summary>
        /// Gets list of all managed exceptions that were *set*. It is hard to understand what exactly 
        /// *set* means in this context, as it returns non-set exception in certain cases. 
        /// </summary>
        private EXCEPTION_INFO[] GetSetManagedExceptions() {
            var guid = VSConstants.DebugEnginesGuids.ManagedOnly_guid;
            return Session.GetSetExceptionsAsArraySafe(null, null, ref guid);
        }

        private ExceptionBreakState GetStateFromSession() {
            var inferredState = ExceptionBreakState.Unknown;
            var exceptionThatCausedChangeFromUnknown = new EXCEPTION_INFO();

            foreach (var exception in GetSetManagedExceptions()) {
                if (_ignorePredicate(exception.bstrExceptionName))
                    continue;

                var @break = (((enum_EXCEPTION_STATE)exception.dwState & VSExceptionStateStopAllInfer) == VSExceptionStateStopAllInfer);
                var stateFromException = @break ? ExceptionBreakState.BreakOnAll : ExceptionBreakState.BreakOnNone;

                if (inferredState == ExceptionBreakState.Unknown) {
                    inferredState = stateFromException;
                    exceptionThatCausedChangeFromUnknown = exception;
                    continue;
                }

                if (inferredState != stateFromException) {
                    _logger.WriteLine("Manager: inconclusive state diagnostic.");
                    _logger.WriteLine("  Previous state:        {0}.", inferredState);
                    _logger.WriteLine("  Previous exception:    {0}; {1}.", exceptionThatCausedChangeFromUnknown.bstrExceptionName, (enum_EXCEPTION_STATE)exceptionThatCausedChangeFromUnknown.dwState);
                    _logger.WriteLine("  Conflicting state:     {0}.", stateFromException);
                    _logger.WriteLine("  Conflicting exception: {0}; {1}.", exception.bstrExceptionName, (enum_EXCEPTION_STATE)exception.dwState);

                    inferredState = ExceptionBreakState.Inconclusive;
                    break;
                }
            }

            _logger.WriteLine("Manager: inferred state is {0}.", inferredState);
            return inferredState;
        }

        private void ApplyStateToSession(ExceptionBreakState state) {
            const int SkippedExceptionLogLimit = 25;

            _logger.WriteLine("Manager: Applying {0} to debug session.", state);
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var newExceptionState = (state == ExceptionBreakState.BreakOnAll)
                                  ? (uint)VSExceptionStateStopAll
                                  : (uint)VSExceptionStateStopNotSet;

            var guid = Guid.Empty;
            var hr = Session.RemoveAllSetExceptions(ref guid);
            if (hr != VSConstants.S_OK)
                Marshal.ThrowExceptionForHR(hr);

            var updated = new EXCEPTION_INFO[1];
            var skippedExceptionCount = 0;
            foreach (var exception in _exceptionCache) {
                if (_ignorePredicate(exception.bstrExceptionName)) {
                    if (skippedExceptionCount < SkippedExceptionLogLimit)
                        _logger.WriteLine("  Skipped exception {0} (matches ignore rule).", exception.bstrExceptionName);

                    skippedExceptionCount += 1;
                    continue;
                }

                updated[0] = new EXCEPTION_INFO {
                    guidType = exception.guidType,
                    bstrExceptionName = exception.bstrExceptionName,
                    dwState = newExceptionState
                };

                hr = Session.SetException(updated);
                if (hr != VSConstants.S_OK)
                    Marshal.ThrowExceptionForHR(hr);
            }

            if (skippedExceptionCount > SkippedExceptionLogLimit)
                _logger.WriteLine("  Skipped {0} more exceptions (match ignore rule).", skippedExceptionCount - SkippedExceptionLogLimit);

            stopwatch.Stop();
            _logger.WriteLine("  Finished in {0}ms.", stopwatch.ElapsedMilliseconds);
        }
    }
}
