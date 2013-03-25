using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BreakOnExceptionsPlus.Implementation {
    public enum ExceptionBreakState {
        Unknown,
        Inconclusive,
        BreakOnAll,
        BreakOnNone
    }
}
