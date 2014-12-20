namespace ExceptionBreaker.Core {
    public interface IDiagnosticLogger {
        void WriteLine(string message);
        void WriteLine(string format, params object[] args);
    }
}