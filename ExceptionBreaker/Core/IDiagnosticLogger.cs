using JetBrains.Annotations;

namespace ExceptionBreaker.Core {
    public interface IDiagnosticLogger {
        void WriteLine(string message);
        [StringFormatMethod("format")]
        void WriteLine(string format, params object[] args);
        [StringFormatMethod("format")]
        void WriteLine(string format, object arg1);
    }
}