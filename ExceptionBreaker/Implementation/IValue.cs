using System;
using System.Collections.Generic;

namespace ExceptionBreaker.Implementation {
    public interface IValue<T> : IReadOnlyValue<T> {
        new T Value { get; set; }
    }
}
