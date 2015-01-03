using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace ExceptionBreaker.Core {
    public class DTEExport {
        [ImportingConstructor]
        public DTEExport(SVsServiceProvider serviceProvider) {
            DTE = (DTE) serviceProvider.GetService(typeof (DTE));
        }

        [Export]
        public DTE DTE { get; private set; }
    }
}
