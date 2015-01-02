using System;
using System.Collections.Generic;
using System.Linq;

namespace ExceptionBreaker.Breakpoints {
    public class BreakpointExceptionSettings {
        public ExceptionBreakChange Change { get; set; }
        public bool ContinueExecution { get; set; }
    }
}
