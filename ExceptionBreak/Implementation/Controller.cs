using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;

namespace ExceptionBreak.Implementation {
    public class Controller {
        private readonly MenuCommand breakOnAllCommand;
        private readonly ExceptionBreakManager breakManager;
        private readonly IDiagnosticLogger logger;

        public Controller(Func<EventHandler, MenuCommand> initBreakOnAllCommand, ExceptionBreakManager breakManager, IDiagnosticLogger logger) {
            this.breakManager = breakManager;
            this.logger = logger;
            this.breakOnAllCommand = initBreakOnAllCommand(breakOnAllCommand_Callback);
            this.breakOnAllCommand.Enabled = true;

            this.breakManager.CurrentStateChanged += breakManager_CurrentStateChanged;
        }

        private void breakOnAllCommand_Callback(object sender, EventArgs e) {
            try {
                var targetState = this.breakManager.CurrentState != ExceptionBreakState.BreakOnAll
                                ? ExceptionBreakState.BreakOnAll
                                : ExceptionBreakState.BreakOnNone;

                this.breakManager.CurrentState = targetState;
            }
            catch (Exception ex) {
                this.logger.WriteLine("Unexpected exception: " + ex);
            }
        }

        private void breakManager_CurrentStateChanged(object sender, EventArgs eventArgs) {
            this.breakOnAllCommand.Checked = (this.breakManager.CurrentState == ExceptionBreakState.BreakOnAll);
        }
    }
}