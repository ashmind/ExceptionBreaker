using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.Shell.Interop;

namespace ExceptionBreaker.Core {
    public class ExtensionLogger : IDiagnosticLogger {
        private readonly IVsOutputWindowPane _outputPane;
        private readonly string _traceCategory;

        public ExtensionLogger(string name, Func<string, IVsOutputWindowPane> getOutputPane) {
            _outputPane = getOutputPane("Ext: " + name + " (Diagnostic)");
            _traceCategory = name;
        }

        public void WriteLine(string message) {
            _outputPane.OutputString(message + Environment.NewLine);
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
    }
}
