using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

// ReSharper disable CheckNamespace
namespace Microsoft.VisualStudio.Debugger.Interop.Internal {
// ReSharper restore CheckNamespace
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct DataTipIdentity {
        [MarshalAs(UnmanagedType.BStr)]
        public string moniker;
        public uint position;
    }
}
