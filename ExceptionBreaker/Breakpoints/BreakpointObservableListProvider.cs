using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using EnvDTE;
using EnvDTE80;
using ExceptionBreaker.Core;
using Microsoft.VisualStudio;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExceptionBreaker.Breakpoints {
    public class BreakpointObservableListProvider {
        private readonly DTE _dte;
        private readonly IDiagnosticLogger _logger;
        private readonly IList<CommandEvents> _commandEvents;

        public BreakpointObservableListProvider(DTE dte, IDiagnosticLogger logger) {
            _dte = dte;
            _logger = logger;

            _commandEvents = new List<CommandEvents>();
            Breakpoints = new ObservableCollection<Breakpoint2>(_dte.Debugger.Breakpoints.Cast<Breakpoint2>());

            SubscribeToCommand(dte, VSConstants.VSStd97CmdID.InsertBreakpoint, InsertBreakpoint_AfterExecute);
        }

        private void SubscribeToCommand(DTE dte, VSConstants.VSStd97CmdID id, _dispCommandEvents_AfterExecuteEventHandler handler) {
            var events = dte.Events.CommandEvents[
                typeof (VSConstants.VSStd97CmdID).GUID.ToString("B"),
                (int)id
            ];
            _commandEvents.Add(events);
            events.AfterExecute += handler;
        }

        private void InsertBreakpoint_AfterExecute(string Guid, int ID, object CustomIn, object CustomOut) {
            _logger.WriteLine("Command: InsertBreakpoint detected.");
            var allBreakpoints = _dte.Debugger.Breakpoints.Cast<Breakpoint2>().ToArray();
        }

        [Export]
        public ObservableCollection<Breakpoint2> Breakpoints { get; private set; } 
    }
}
