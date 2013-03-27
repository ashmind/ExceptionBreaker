using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.Shell.Interop;

namespace ExceptionBreaker.Implementation {
    /// <summary>
    /// Logs same information to Trace of the debugging VS and Output window for the target VS.
    /// </summary>
    public class DiagnosticLogger : IDiagnosticLogger {
        private readonly IVsOutputWindowPane output;
        private readonly string traceCategory;

        public DiagnosticLogger(IVsOutputWindowPane output, string traceCategory) {
            this.output = output;
            this.traceCategory = traceCategory;
        }

        public void WriteLine(string message) {
            this.output.OutputString(message + Environment.NewLine);
            Trace.WriteLine(message, this.traceCategory);
        }

        public void WriteLine(string format, params object[] args) {
            this.WriteLine(string.Format(format, args));
        }
    }
}
