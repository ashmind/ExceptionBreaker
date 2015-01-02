using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using EnvDTE80;
using ExceptionBreaker.Core;
using JetBrains.Annotations;
using Microsoft.VisualStudio.Debugger.Interop;

namespace ExceptionBreaker.Breakpoints {
    public class BreakpointKeyProvider {
        [NotNull]
        public string GetKey(Breakpoint2 breakpoint) {
            return string.Intern(breakpoint.File + ":" + breakpoint.FileLine + ":" + breakpoint.FileColumn);
        }

        [CanBeNull]
        public string GetKey(IDebugBoundBreakpoint2 breakpoint) {
            var resolution = breakpoint.GetBreakpointResolutionSafe();
            var resolutionInfo = resolution.GetResolutionInfoSafe((int)enum_BPRESI_FIELDS.BPRESI_BPRESLOCATION);
            var location = resolutionInfo.bpResLocation;
            if (location.bpType != (uint)enum_BP_TYPE.BPT_CODE)
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
