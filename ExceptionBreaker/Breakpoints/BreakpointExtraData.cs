using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExceptionBreaker.Core.Observable;

namespace ExceptionBreaker.Breakpoints {
    public class BreakpointExtraData {
        public BreakpointExtraData() {
            ExceptionBreakChange = new ObservableValue<ExceptionBreakChange>();
        }

        public ObservableValue<ExceptionBreakChange> ExceptionBreakChange { get; private set; }
    }
}
