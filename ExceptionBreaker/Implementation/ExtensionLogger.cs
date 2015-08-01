using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace ExceptionBreaker.Implementation {
    public class ExtensionLogger : IDiagnosticLogger {
        private readonly string _traceCategory;
        private readonly IServiceProvider _serviceProvider;
        private readonly Guid _outputPaneGuid;
        private readonly string _outputPaneCaption;

        public ExtensionLogger(string name, IServiceProvider serviceProvider, Guid outputPaneGuid) {
            _traceCategory = name;
            _serviceProvider = serviceProvider;
            _outputPaneGuid = outputPaneGuid;
            _outputPaneCaption = "Ext: " + _traceCategory + " (Diagnostic)";
        }

        public void WriteLine(string message) {
            var outputPane = GetOutputPane();
            if (outputPane != null)
                outputPane.OutputString(message + Environment.NewLine);

            Trace.WriteLine(message, _traceCategory);
        }

        public void WriteLine(string format, params object[] args) {
            WriteLine(string.Format(format, args));
        }

        public void WriteLine(string format, object arg1) {
            WriteLine(string.Format(format, arg1));
        }

        public void WriteLine(string format, object arg1, object arg2) {
            WriteLine(string.Format(format, arg1, arg2));
        }
        
        private IVsOutputWindowPane GetOutputPane() {
            var outputWindow = (IVsOutputWindow)_serviceProvider.GetService(typeof(SVsOutputWindow));
            if (outputWindow == null)
                return null;
            
            var guid = _outputPaneGuid;
            var pane = (IVsOutputWindowPane)null;
            var hr = outputWindow.GetPane(ref guid, out pane);
            if (hr != VSConstants.E_FAIL && hr != VSConstants.E_INVALIDARG)
                VSInteropHelper.Validate(hr);

            if (pane == null) {
                VSInteropHelper.Validate(outputWindow.CreatePane(ref guid, _outputPaneCaption, 1, 1));
                VSInteropHelper.Validate(outputWindow.GetPane(ref guid, out pane));
            }

            VSInteropHelper.Validate(pane.Activate());
            return pane;
        }
    }
}