using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Debugger.Interop.Internal;
using Microsoft.VisualStudio.Shell.Interop;

namespace ExceptionBreaker.Core.VersionSpecific {
    public class VersionSpecificAdapterExport {
        [ImportingConstructor]
        public VersionSpecificAdapterExport(DTE dte, IVsDebugger debugger) {
            var version = new Version(dte.Version);
            Adapter = AdaptDebuggerInternal(debugger, version);
        }

        [Export]
        public IDebuggerInternalAdapter Adapter { get; private set; }

        private static IDebuggerInternalAdapter AdaptDebuggerInternal(object debugger, Version version) {
            if (version.Major >= 11) 
                return new DebuggerInternal11Adapter((IDebuggerInternal11)debugger);

            if (version.Major == 10)
                return new DebuggerInternal10Adapter((IDebuggerInternal10)debugger);

            throw new NotSupportedException("Visual Studio version " + version + " is not supported.");
        }
    }
}
