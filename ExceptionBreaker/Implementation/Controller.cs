using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio;

namespace ExceptionBreaker.Implementation {
    public class Controller {
        private readonly MenuCommand breakOnAllCommand;
        private readonly ExceptionBreakManager breakManager;
        private readonly IDiagnosticLogger logger;
        private readonly CommandEvents debugExceptionsEvents;

        public Controller(DTE dte,
                          Func<EventHandler, MenuCommand> initBreakOnAllCommand,
                          ExceptionBreakManager breakManager,
                          IDiagnosticLogger logger)
        {
            this.breakManager = breakManager;
            this.logger = logger;
            this.breakOnAllCommand = initBreakOnAllCommand(breakOnAllCommand_Callback);

            this.breakManager.CurrentStateChanged += breakManager_CurrentStateChanged;

            this.debugExceptionsEvents = dte.Events.CommandEvents[
                typeof(VSConstants.VSStd97CmdID).GUID.ToString("B"),
                (int)VSConstants.VSStd97CmdID.Exceptions
            ];
            this.debugExceptionsEvents.AfterExecute += debugExceptionsEvents_AfterExecute;
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
            var enabledVisible = (this.breakManager.CurrentState != ExceptionBreakState.Unknown);
            var @checked = (this.breakManager.CurrentState == ExceptionBreakState.BreakOnAll);

            this.logger.WriteLine("Command: change of state, enabled = {0}, visible = {0}, checked = {1}.", enabledVisible, @checked);
            var command = this.breakOnAllCommand;
            command.Enabled = enabledVisible;
            command.Visible = enabledVisible;
            command.Checked = @checked;
        }
    }
}