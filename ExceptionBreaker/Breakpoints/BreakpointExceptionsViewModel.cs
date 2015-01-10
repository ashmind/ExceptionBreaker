using System;
using System.Collections.Generic;
using System.Linq;
using ExceptionBreaker.Core.Observable;

namespace ExceptionBreaker.Breakpoints {
    public class BreakpointExceptionsViewModel {
        public BreakpointExceptionsViewModel() {
            ShouldChange = new ObservableValue<bool>();
        }

        public ObservableValue<bool> ShouldChange { get; private set; }
        public ExceptionBreakChange Change { get; set; }
        public bool ContinueExecution { get; set; }
    }
}
