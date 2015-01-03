using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace ExceptionBreaker.Core {
    [Export]
    public class DTEImport {
        [ImportingConstructor]
        public DTEImport(SVsServiceProvider serviceProvider) {
            DTE = (DTE) serviceProvider.GetService(typeof (DTE));
        }

        public DTE DTE { get; private set; }
    }
}
