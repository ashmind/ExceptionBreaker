using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable CheckNamespace
namespace Microsoft.VisualStudio.Debugger.Interop.Internal {
// ReSharper restore CheckNamespace
    [ComImport, Guid("405E9EA6-4CD0-404C-927F-477BF8E09696"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBreakpointEvents {
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int Changed();
    }
}