using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace ExceptionBreaker.Core {
    public class VsDebuggerExport {
        [ImportingConstructor]
        public VsDebuggerExport(SVsServiceProvider serviceProvider) {
            Debugger = (IVsDebugger)serviceProvider.GetService(typeof(SVsShellDebugger));
        }

        [Export]
        public IVsDebugger Debugger { get; private set; }
    }
}
