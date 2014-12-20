using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace ExceptionBreaker.Implementation {
    /// <summary>
    /// Handles state (enabled, visible, checked) and click events for the BreakOnAllCommand.
    /// </summary>
    public class CommandController : IVsSelectionEvents, IDisposable {
        private static readonly HashSet<Guid> RequiredUIContexts = new HashSet<Guid> {
            VSConstants.UICONTEXT.Debugging_guid,
            VSConstants.UICONTEXT.SolutionExists_guid
        };

        private readonly MenuCommand _breakOnAllCommand;
        private readonly IVsMonitorSelection _monitorSelection;
        private readonly ExceptionBreakManager _breakManager;
        private readonly IDiagnosticLogger _logger;
        private readonly CommandEvents _debugExceptionsEvents;

        private readonly uint _selectionEventsCookie;

        private readonly HashSet<uint> _requiredUiContextCookies;
        private readonly HashSet<uint> _currentlyActiveUiContextCookies = new HashSet<uint>();

        public CommandController(DTE dte,
                                 Func<EventHandler, MenuCommand> initBreakOnAllCommand,
                                 IVsMonitorSelection monitorSelection,
                                 ExceptionBreakManager breakManager,
                                 IDiagnosticLogger logger)
        {
            _monitorSelection = monitorSelection;
            _breakManager = breakManager;
            _logger = logger;
            _breakOnAllCommand = initBreakOnAllCommand(breakOnAllCommand_Callback);

            _requiredUiContextCookies = new HashSet<uint>(RequiredUIContexts.Select(ConvertToUIContextCookie));

            UpdateCommandAvailability();
            _selectionEventsCookie = SubscribeToSelectionEvents();

            UpdateCommandCheckedState();
            _breakManager.CurrentStateChanged += breakManager_CurrentStateChanged;

            _debugExceptionsEvents = SubscribeToDebugExceptionsCommand(dte);
        }

        private uint SubscribeToSelectionEvents() {
            uint cookie;
            var hr = _monitorSelection.AdviseSelectionEvents(this, out cookie);
            if (hr != VSConstants.S_OK)
                Marshal.ThrowExceptionForHR(hr);

            return cookie;
        }

        private CommandEvents SubscribeToDebugExceptionsCommand(DTE dte) {
            var events = dte.Events.CommandEvents[
                typeof(VSConstants.VSStd97CmdID).GUID.ToString("B"),
                (int)VSConstants.VSStd97CmdID.Exceptions
            ];
            events.AfterExecute += debugExceptionsEvents_AfterExecute;

            return events;
        }

        private uint ConvertToUIContextCookie(Guid guid) {
            uint cookie;
            var hr = _monitorSelection.GetCmdUIContextCookie(ref guid, out cookie);
            if (hr != VSConstants.S_OK)
                Marshal.ThrowExceptionForHR(hr);

            _logger.WriteLine("Mapped UI context {0} to cookie {1}.", guid, cookie);
            return cookie;
        }

        private void debugExceptionsEvents_AfterExecute(string Guid, int ID, object CustomIn, object CustomOut) {
            _logger.WriteLine("Debug.Exceptions was just executed.");
            _breakManager.RefreshCurrentState();
        }

        private void breakOnAllCommand_Callback(object sender, EventArgs e) {
            try {
                var targetState = _breakManager.CurrentState != ExceptionBreakState.BreakOnAll
                                ? ExceptionBreakState.BreakOnAll
                                : ExceptionBreakState.BreakOnNone;

                _logger.WriteLine("Command: toggled, current = {0}, new = {1}.", _breakManager.CurrentState, targetState);
                _breakManager.CurrentState = targetState;
            }
            catch (Exception ex) {
                _logger.WriteLine("Unexpected exception: " + ex);
                MessageBox.Show(ex.Message, "Error in ExceptionBreaker extension", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void breakManager_CurrentStateChanged(object sender, EventArgs eventArgs) {
            UpdateCommandCheckedState();
        }

        public int OnCmdUIContextChanged(uint dwCmdUICookie, int fActive) {
            try {
                var active = (fActive != 0);
                if (active) {
                    _currentlyActiveUiContextCookies.Add(dwCmdUICookie);
                }
                else {
                    _currentlyActiveUiContextCookies.Remove(dwCmdUICookie);
                }

                UpdateCommandAvailability();
                return VSConstants.S_OK;
            }
            catch (Exception ex) {
                _logger.WriteLine("Unexpected exception: " + ex);
                return VSConstants.E_FAIL;
            }
        }

        private void UpdateCommandAvailability() {
            var enabledVisible = _requiredUiContextCookies.Intersect(_currentlyActiveUiContextCookies).Any();

            var command = _breakOnAllCommand;
            if (command.Enabled == enabledVisible) // Visible is always synchronized
                return;

            command.Enabled = enabledVisible;
            command.Visible = enabledVisible;
            _logger.WriteLine("Command: change of state, enabled = {0}, visible = {0}.", enabledVisible);
        }

        private void UpdateCommandCheckedState() {
            var @checked = (_breakManager.CurrentState == ExceptionBreakState.BreakOnAll);
            if (_breakOnAllCommand.Checked == @checked)
                return;

            _breakOnAllCommand.Checked = @checked;
            _logger.WriteLine("Command: change of state, checked = {0}.", @checked);
        }

        public void Dispose() {
            // not completely implemented, please ignore for now
            _monitorSelection.UnadviseSelectionEvents(_selectionEventsCookie);

            _debugExceptionsEvents.AfterExecute -= debugExceptionsEvents_AfterExecute;
            _breakManager.CurrentStateChanged -= breakManager_CurrentStateChanged;
        }

        #region IVsSelectionEvents Members

        int IVsSelectionEvents.OnElementValueChanged(uint elementid, object varValueOld, object varValueNew) {
            return VSConstants.S_OK;
        }

        int IVsSelectionEvents.OnSelectionChanged(IVsHierarchy pHierOld, uint itemidOld, IVsMultiItemSelect pMISOld, ISelectionContainer pSCOld, IVsHierarchy pHierNew, uint itemidNew, IVsMultiItemSelect pMISNew, ISelectionContainer pSCNew) {
            return VSConstants.S_OK;
        }

        #endregion
    }
}