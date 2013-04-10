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

        private readonly MenuCommand breakOnAllCommand;
        private readonly IVsMonitorSelection monitorSelection;
        private readonly ExceptionBreakManager breakManager;
        private readonly IDiagnosticLogger logger;
        private readonly CommandEvents debugExceptionsEvents;

        private readonly uint selectionEventsCookie;

        private readonly HashSet<uint> requiredUIContextCookies;
        private readonly HashSet<uint> currentlyActiveUIContextCookies = new HashSet<uint>();

        public CommandController(DTE dte,
                                 Func<EventHandler, MenuCommand> initBreakOnAllCommand,
                                 IVsMonitorSelection monitorSelection,
                                 ExceptionBreakManager breakManager,
                                 IDiagnosticLogger logger)
        {
            this.monitorSelection = monitorSelection;
            this.breakManager = breakManager;
            this.logger = logger;
            this.breakOnAllCommand = initBreakOnAllCommand(breakOnAllCommand_Callback);

            this.selectionEventsCookie = this.SubscribeToSelectionEvents();
            this.requiredUIContextCookies = new HashSet<uint>(RequiredUIContexts.Select(ConvertToUIContextCookie));

            this.breakManager.CurrentStateChanged += breakManager_CurrentStateChanged;

            this.debugExceptionsEvents = this.SubscribeToDebugExceptionsCommand(dte);
        }

        private uint SubscribeToSelectionEvents() {
            uint cookie;
            var hr = this.monitorSelection.AdviseSelectionEvents(this, out cookie);
            if (hr != VSConstants.S_OK)
                Marshal.ThrowExceptionForHR(hr);

            return cookie;
        }

        private CommandEvents SubscribeToDebugExceptionsCommand(DTE dte) {
            var events = dte.Events.CommandEvents[
                typeof(VSConstants.VSStd97CmdID).GUID.ToString("B"),
                (int)VSConstants.VSStd97CmdID.Exceptions
            ];
            events.AfterExecute += this.debugExceptionsEvents_AfterExecute;

            return events;
        }

        private uint ConvertToUIContextCookie(Guid guid) {
            uint cookie;
            var hr = this.monitorSelection.GetCmdUIContextCookie(ref guid, out cookie);
            if (hr != VSConstants.S_OK)
                Marshal.ThrowExceptionForHR(hr);

            this.logger.WriteLine("Mapped UI context {0} to cookie {1}.", guid, cookie);
            return cookie;
        }

        private void debugExceptionsEvents_AfterExecute(string Guid, int ID, object CustomIn, object CustomOut) {
            this.logger.WriteLine("Debug.Exceptions was just executed.");
            this.breakManager.RefreshCurrentState();
        }

        private void breakOnAllCommand_Callback(object sender, EventArgs e) {
            try {
                var targetState = this.breakManager.CurrentState != ExceptionBreakState.BreakOnAll
                                ? ExceptionBreakState.BreakOnAll
                                : ExceptionBreakState.BreakOnNone;

                this.logger.WriteLine("Command: toggled, current = {0}, new = {1}.", this.breakManager.CurrentState, targetState);
                this.breakManager.CurrentState = targetState;
            }
            catch (Exception ex) {
                this.logger.WriteLine("Unexpected exception: " + ex);
                MessageBox.Show(ex.Message, "Error in ExceptionBreaker extension", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void breakManager_CurrentStateChanged(object sender, EventArgs eventArgs) {
            var @checked = (this.breakManager.CurrentState == ExceptionBreakState.BreakOnAll);
            if (this.breakOnAllCommand.Checked == @checked)
                return;
            
            this.breakOnAllCommand.Checked = @checked;
            this.logger.WriteLine("Command: change of state, checked = {0}.", @checked);
        }

        public int OnCmdUIContextChanged(uint dwCmdUICookie, int fActive) {
            try {
                this.logger.WriteLine("Command UI context changed: cookie = {0} set to {1}.", dwCmdUICookie, fActive);

                var active = (fActive != 0);
                if (active) {
                    this.currentlyActiveUIContextCookies.Add(dwCmdUICookie);
                }
                else {
                    this.currentlyActiveUIContextCookies.Remove(dwCmdUICookie);
                }

                UpdateCommandAvailability();
                return VSConstants.S_OK;
            }
            catch (Exception ex) {
                this.logger.WriteLine("Unexpected exception: " + ex);
                return VSConstants.E_FAIL;
            }
        }

        private void UpdateCommandAvailability() {
            var enabledVisible = this.requiredUIContextCookies.Intersect(this.currentlyActiveUIContextCookies).Any();

            var command = this.breakOnAllCommand;
            if (command.Enabled == enabledVisible) // Visible is always synchronized
                return;
            
            command.Enabled = enabledVisible;
            command.Visible = enabledVisible;
            this.logger.WriteLine("Command: change of state, enabled = {0}, visible = {0}.", enabledVisible);
        }

        public void Dispose() {
            // not completely implemented, please ignore for now
            this.monitorSelection.UnadviseSelectionEvents(this.selectionEventsCookie);

            this.debugExceptionsEvents.AfterExecute -= debugExceptionsEvents_AfterExecute;
            this.breakManager.CurrentStateChanged -= breakManager_CurrentStateChanged;
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