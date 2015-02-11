using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using AshMind.Extensions;
using EnvDTE;
using EnvDTE80;
using ExceptionBreaker.Core;
using ExceptionBreaker.Core.VersionSpecific;
using Microsoft.VisualStudio;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Debugger.Interop.Internal;
using Microsoft.VisualStudio.Text;

namespace ExceptionBreaker.Breakpoints {
    [Export]
    public class BreakpointObservableListProvider : IBreakpointEvents, IDisposable {
        private static readonly ISet<Breakpoint2> NoBreakpoints = new HashSet<Breakpoint2>();

        private readonly DTE _dte;
        private readonly IBreakpointManager _breakpointManager;
        private readonly IDiagnosticLogger _logger;

        [ImportingConstructor]
        public BreakpointObservableListProvider(DTE dte, IDebuggerInternalAdapter debugger, IDiagnosticLogger logger) {
            _dte = dte;
            _breakpointManager = debugger.BreakpointManager;
            _logger = logger;

            Breakpoints = new ObservableCollection<Breakpoint2>();
            var hr = _breakpointManager.AdviseEvents(this);
            VSInteropHelper.Validate(hr);
        }
        
        [Export]
        public ObservableCollection<Breakpoint2> Breakpoints { get; private set; }

        int IBreakpointEvents.Changed() {
            _logger.WriteLine("Event: Breakpoints changed.");

            var collection = _dte.Debugger.Breakpoints;
            var allBreakpoints = collection != null ? collection.Cast<Breakpoint2>().ToSet() : NoBreakpoints;
            Breakpoints.RemoveWhere(b => !allBreakpoints.Contains(b));
            Breakpoints.AddRange(allBreakpoints.Except(Breakpoints));

            return VSConstants.S_OK;
        }

        public void Dispose() {
            var hr = _breakpointManager.UnadviseEvents(this);
            VSInteropHelper.Validate(hr);
        }
    }
}
