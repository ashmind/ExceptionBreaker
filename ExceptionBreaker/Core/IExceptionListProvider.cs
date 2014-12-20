using System;
using System.Collections.Generic;

namespace ExceptionBreaker.Core {
    public interface IExceptionListProvider {
        event EventHandler ExceptionNamesChanged;
        IEnumerable<string> GetExceptionNames();
    }
}