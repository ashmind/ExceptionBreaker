using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExceptionBreaker.Breakpoints {
    public class BreakpointExtraData {
        public BreakpointExtraData() {
            Version = 1; // for serialization tracking
        }

        public int Version { get; set; }
        public ExceptionBreakChange ExceptionBreakChange { get; set; }
    }
}
