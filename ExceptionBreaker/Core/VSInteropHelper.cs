using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;

namespace ExceptionBreaker.Core {
    public static class VSInteropHelper {
        public static void Validate(int hresult) {
            if (hresult != VSConstants.S_OK)
                Marshal.ThrowExceptionForHR(hresult);
        }

        public static void Release(object comObject) {
            if (comObject == null)
                return;
            
            Marshal.ReleaseComObject(comObject);
        }
    }
}