using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Shell;

namespace ExceptionBreaker.Core {
    public class ExtensionLoggerExport {
        [ImportingConstructor]
        public ExtensionLoggerExport(SVsServiceProvider serviceProvider) {
            Logger = new ExtensionLogger("ExceptionBreaker", serviceProvider, GuidList.OutputPane);
        }

        [Export(typeof(IDiagnosticLogger))]
        public IDiagnosticLogger Logger { get; private set; }
    }
}
