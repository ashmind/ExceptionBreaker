namespace ExceptionBreaker.Implementation {
    public interface IReadOnlyValue<T> {
        T Value { get; }
    }
}