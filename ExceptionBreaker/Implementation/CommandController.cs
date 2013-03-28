using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio;

namespace ExceptionBreaker.Implementation {
    public class CommandController : IDisposable {
        private readonly MenuCommand breakOnAllCommand;
        private readonly ExceptionBreakManager breakManager;
        private readonly DebugSessionWatcher sessionWatcher;
        private readonly IDiagnosticLogger logger;
        private readonly CommandEvents debugExceptionsEvents;

        public CommandController(DTE dte,
                                 Func<EventHandler, MenuCommand> initBreakOnAllCommand,
                                 ExceptionBreakManager breakManager,
                                 DebugSessionWatcher sessionWatcher,
                                 IDiagnosticLogger logger)
        {
            this.breakManager = breakManager;
            this.sessionWatcher = sessionWatcher;
            this.logger = logger;
            this.breakOnAllCommand = initBreakOnAllCommand(breakOnAllCommand_Callback);

            this.sessionWatcher.DebugSessionChanged += sessionWatcher_DebugSessionChanged;
            this.breakManager.CurrentStateChanged += breakManager_CurrentStateChanged;

            this.debugExceptionsEvents = this.SubscribeToDebugExceptionsCommand(dte);
        }

        private CommandEvents SubscribeToDebugExceptionsCommand(DTE dte) {
            var events = dte.Events.CommandEvents[
                typeof(VSConstants.VSStd97CmdID).GUID.ToString("B"),
                (int)VSConstants.VSStd97CmdID.Exceptions
            ];
            events.AfterExecute += this.debugExceptionsEvents_AfterExecute;

            return events;
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

        private void sessionWatcher_DebugSessionChanged(object sender, EventArgs e) {
            var enabledVisible = (this.sessionWatcher.DebugSession != null);

            var command = this.breakOnAllCommand;
            if (command.Enabled == enabledVisible) // Visible is always synchronized
                return;

            command.Enabled = enabledVisible;
            command.Visible = enabledVisible;
            this.logger.WriteLine("Command: change of state, enabled = {0}, visible = {0}.", enabledVisible);
        }

        private void breakManager_CurrentStateChanged(object sender, EventArgs eventArgs) {
            var @checked = (this.breakManager.CurrentState == ExceptionBreakState.BreakOnAll);
            if (this.breakOnAllCommand.Checked == @checked)
                return;
            
            this.breakOnAllCommand.Checked = @checked;
            this.logger.WriteLine("Command: change of state, checked = {0}.", @checked);
        }

        public void Dispose() {
            // not completely implemented, please ignore for now
            this.debugExceptionsEvents.AfterExecute -= debugExceptionsEvents_AfterExecute;
            this.sessionWatcher.DebugSessionChanged -= sessionWatcher_DebugSessionChanged;
            this.breakManager.CurrentStateChanged -= breakManager_CurrentStateChanged;
        }
    }
}