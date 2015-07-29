using JetBrains.Annotations;

namespace ExceptionBreaker.Implementation {
    public interface IDiagnosticLogger {
        void WriteLine(string message);
        void WriteLine(string format, params object[] args);
        void WriteLine(string format, object arg1);
        void WriteLine(string format, object arg1, object arg2);
    }
}