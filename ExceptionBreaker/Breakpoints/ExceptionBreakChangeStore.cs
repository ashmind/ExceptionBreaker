using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using EnvDTE80;
using ExceptionBreaker.Core;
using Microsoft.VisualStudio.Debugger.Interop;

namespace ExceptionBreaker.Breakpoints {
    public class ExceptionBreakChangeStore {
        private readonly IDiagnosticLogger _logger;
        private readonly IDictionary<string, ExceptionBreakChange> _store = new Dictionary<string, ExceptionBreakChange>(StringComparer.InvariantCultureIgnoreCase);

        public ExceptionBreakChangeStore(IDiagnosticLogger logger) {
            _logger = logger;
        }

        public ExceptionBreakChange GetChange(Breakpoint2 breakpoint) {
            return GetChange(GetKey(breakpoint));
        }

        public ExceptionBreakChange GetChange(IDebugBoundBreakpoint2 breakpoint) {
            return GetChange(GetKey(breakpoint));
        }

        private ExceptionBreakChange GetChange(string key) {
            ExceptionBreakChange value;
            if (key == null || !_store.TryGetValue(key, out value))
                value = ExceptionBreakChange.NoChange;

            _logger.WriteLine("Breakpoint '{0}' setting loaded as {1}.", key, value);
            return value;
        }

        public void SetChange(Breakpoint2 breakpoint, ExceptionBreakChange value) {
            var key = GetKey(breakpoint);

            _store[key] = value;
            _logger.WriteLine("Breakpoint '{0}' setting changed to {1}.", key, value);
        }

        private string GetKey(Breakpoint2 breakpoint) {
            return string.Intern(breakpoint.File + ":" + breakpoint.FileLine + ":" + breakpoint.FileColumn);
        }

        private string GetKey(IDebugBoundBreakpoint2 breakpoint) {
            var resolution = breakpoint.GetBreakpointResolutionSafe();
            var resolutionInfo = resolution.GetResolutionInfoSafe((int)enum_BPRESI_FIELDS.BPRESI_BPRESLOCATION);
            var location = resolutionInfo.bpResLocation;
            if (location.bpType != (uint) enum_BP_TYPE.BPT_CODE)
                return null;

            var context = (IDebugCodeContext2)Marshal.GetObjectForIUnknown(location.unionmember1);
            var documentContext = context.GetDocumentContextSafe();
            var fileName = documentContext.GetNameSafe((uint)enum_GETNAME_TYPE.GN_FILENAME);
            
            var position = new TEXT_POSITION[1];
            var hr = documentContext.GetStatementRange(position, null);
            VSInteropHelper.Validate(hr);

            return string.Intern(fileName + ":" + (position[0].dwLine + 1) + ":" + (position[0].dwColumn + 1));
        }
    }
}
