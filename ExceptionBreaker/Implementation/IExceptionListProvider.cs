using System;
using System.Collections.Generic;

namespace ExceptionBreaker.Implementation {
    public interface IExceptionListProvider {
        event EventHandler ExceptionNamesChanged;
        IEnumerable<string> GetExceptionNames();
    }
}