using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Windows;
using EnvDTE80;
using ExceptionBreaker.Core;

namespace ExceptionBreaker.Breakpoints {
    public class BreakpointSetupExceptionsController {
        private readonly BreakpointFinder _breakpointFinder;
        private readonly BreakpointExtraDataProvider _extraDataProvider;
        private readonly IDiagnosticLogger _logger;
        private readonly MenuCommand _command;

        public BreakpointSetupExceptionsController(CommandInitializer commandInitializer,
                                                   BreakpointFinder breakpointFinder,
                                                   BreakpointExtraDataProvider extraDataProvider,
                                                   IDiagnosticLogger logger) 
        {
            _breakpointFinder = breakpointFinder;
            _extraDataProvider = extraDataProvider;
            _logger = logger;
            _command = commandInitializer.InitializeCommand(command_Callback, command_BeforeQueryStatus);
        }

        private void command_BeforeQueryStatus(object sender, EventArgs e) {
            try {
                var breakpoint = _breakpointFinder.GetBreakpointForCommand();
                var extraData = _extraDataProvider.GetData(breakpoint);

                var @checked = (extraData != null && extraData.ExceptionBreakChange != ExceptionBreakChange.NoChange);
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
            var extraData = _extraDataProvider.GetData(breakpoint);
            var dialog = new BreakpointExceptionsDialog {
                ViewModel = new BreakpointExceptionSettings {
                    Change = extraData != null ? extraData.ExceptionBreakChange : ExceptionBreakChange.NoChange,
                    ContinueExecution = !breakpoint.BreakWhenHit
                }
            };
            var result = dialog.ShowModal() ?? false;
            if (!result) {
                _logger.WriteLine("User has cancelled breakpoint exception settings dialog.");
                return;
            }

            _logger.WriteLine("Updating breakpoint settings: continue execution = {0}, change = {1}.", dialog.ViewModel.ContinueExecution, dialog.ViewModel.Change);
            extraData.ExceptionBreakChange = dialog.ViewModel.Change;
            breakpoint.BreakWhenHit = !dialog.ViewModel.ContinueExecution;
        }
    }
}
