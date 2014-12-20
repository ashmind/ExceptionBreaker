using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Debugger.Interop.Internal;

namespace ExceptionBreaker.Implementation.VersionSpecific {
    public class VersionSpecificAdapterFactory {
        private readonly Version _version;

        public VersionSpecificAdapterFactory(DTE dte) {
            _version = new Version(dte.Version);
        }

        public IDebuggerInternalAdapter AdaptDebuggerInternal(object debugger) {
            if (_version.Major >= 11) 
                return new DebuggerInternal11Adapter((IDebuggerInternal11)debugger);

            if (_version.Major == 10)
                return new DebuggerInternal10Adapter((IDebuggerInternal10)debugger);

            throw new NotSupportedException("Visual Studio version " + _version + " is not supported.");
        }
    }
}
