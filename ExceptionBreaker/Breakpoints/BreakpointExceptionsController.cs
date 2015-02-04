using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Windows;
using EnvDTE80;
using ExceptionBreaker.Core;

namespace ExceptionBreaker.Breakpoints {
    public class BreakpointExceptionsController {
        private readonly BreakpointFinder _breakpointFinder;
        private readonly BreakpointExtraDataStore _extraDataStore;
        private readonly IDiagnosticLogger _logger;
        private readonly MenuCommand _command;
        
        public BreakpointExceptionsController(CommandInitializer commandInitializer,
                                              BreakpointFinder breakpointFinder,
                                              BreakpointExtraDataStore extraDataStore,
                                              IDiagnosticLogger logger) 
        {
            _breakpointFinder = breakpointFinder;
            _extraDataStore = extraDataStore;
            _logger = logger;
            _command = commandInitializer.InitializeCommand(command_Callback, command_BeforeQueryStatus);
        }

        private void command_BeforeQueryStatus(object sender, EventArgs e) {
            try {
                var breakpoint = _breakpointFinder.GetBreakpointForCommand();
                var extraData = _extraDataStore.GetData(breakpoint);

                var @checked = extraData.ExceptionBreakChange.Value != ExceptionBreakChange.NoChange;
                if (@checked == _command.Checked)
                    return;

                _logger.WriteLine("Breakpoint context menu command: change of state, checked = {0}.", @checked);
                _command.Checked = @checked;
            }
            catch (Exception ex) {
                _logger.WriteLine("Unexpected exception: {0}", ex);
            }
        }

        private void command_Callback(object sender, EventArgs e) {
            try {
                _logger.WriteLine("Breakpoint context menu command.");

                var breakpoint = _breakpointFinder.GetBreakpointForCommand();
                RequestAndUpdateExceptionSettings(breakpoint);
            }
            catch (Exception ex) {
                _logger.WriteLine("Unexpected exception: {0}", ex);
                MessageBox.Show(ex.Message, "Error in ExceptionBreaker extension", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RequestAndUpdateExceptionSettings(Breakpoint2 breakpoint) {
            var extraData = _extraDataStore.GetData(breakpoint);
            var dialog = new BreakpointExceptionsDialog {
                ViewModel = new BreakpointExceptionsViewModel {
                    ShouldChange = { Value = extraData.ExceptionBreakChange.Value != ExceptionBreakChange.NoChange },
                    Change = extraData.ExceptionBreakChange.Value,
                    ContinueExecution = !breakpoint.BreakWhenHit
                }
            };
            var result = dialog.ShowModal() ?? false;
            if (!result) {
                _logger.WriteLine("User has cancelled breakpoint exception settings dialog.");
                return;
            }

            var change = dialog.ViewModel.ShouldChange.Value ? dialog.ViewModel.Change : ExceptionBreakChange.NoChange;
            _logger.WriteLine("Updating breakpoint settings: continue execution = {0}, change = {1}.", dialog.ViewModel.ContinueExecution, change);
            extraData.ExceptionBreakChange.Value = change;

            SetBreakWhenHit(breakpoint, !dialog.ViewModel.ContinueExecution);
            _extraDataStore.NotifyDataChanged(breakpoint);
        }

        private static void SetBreakWhenHit(Breakpoint2 breakpoint, bool value) {
            var messageStubbed = false;
            if (value && string.IsNullOrEmpty(breakpoint.Message)) {
                // http://stackoverflow.com/questions/27753513/visual-studio-sdk-breakpoint2-breakwhenhit-true-throws-exception-0x8971101a/27870066#27870066
                breakpoint.Message = "stub";
                messageStubbed = true;
            }
            breakpoint.BreakWhenHit = value;
            if (messageStubbed)
                breakpoint.Message = "";
        }
    }
}
