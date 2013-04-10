using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable CheckNamespace
namespace Microsoft.VisualStudio.Debugger.Interop.Internal {
// ReSharper restore CheckNamespace
    [ComImport, Guid("17AE185F-980F-4262-A3F0-0D3DEB2CF79A"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBreakpoint {
        [DispId(0x60010000)]
        IDebugCodeContext2 CodeContext { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }
        [DispId(0x60010001), ComAliasName("Microsoft.VisualStudio.Debugger.Interop.Internal.EBreakpointType")]
        EBreakpointType Type { [return: ComAliasName("Microsoft.VisualStudio.Debugger.Interop.Internal.EBreakpointType")] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }
        [DispId(0x60010002)]
        IBreakpoint Parent { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }
        [DispId(0x60010003)]
        int IsEnabled { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }
        [DispId(0x60010004)]
        Array Children { [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UNKNOWN)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }
    }
}