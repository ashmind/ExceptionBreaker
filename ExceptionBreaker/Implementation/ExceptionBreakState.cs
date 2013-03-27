using System;
using System.Collections.Generic;
using System.Linq;

namespace ExceptionBreaker.Implementation {
    public enum ExceptionBreakState {
        Unknown,
        Inconclusive,
        BreakOnAll,
        BreakOnNone
    }
}
