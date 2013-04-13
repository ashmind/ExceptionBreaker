using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Debugger.Interop.Internal;

namespace ExceptionBreaker.Implementation.VersionSpecific {
    public class VersionSpecificAdapterFactory {
        private readonly Version version;

        public VersionSpecificAdapterFactory(DTE dte) {
            this.version = new Version(dte.Version);
        }

        public IDebuggerInternalAdapter AdaptDebuggerInternal(object debugger) {
            if (version.Major == 11) 
                return new DebuggerInternal11Adapter((IDebuggerInternal11)debugger);

            if (version.Major == 10)
                return new DebuggerInternal10Adapter((IDebuggerInternal10)debugger);

            throw new NotSupportedException("Visual Studio version " + this.version + " is not supported.");
        }
    }
}
