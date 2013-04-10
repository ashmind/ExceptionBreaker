using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable CheckNamespace
namespace Microsoft.VisualStudio.Debugger.Interop.Internal {
// ReSharper restore CheckNamespace
    [ComImport, Guid("9499FBA0-8C9D-49F1-B130-A25DED8A89DE"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBreakpointManager {
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int ToggleBreakpointsAtCodeContext([In, MarshalAs(UnmanagedType.Interface)] IDebugCodeContext2 pCodeContext);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int InsertBreakpointAtCodeContext([In, MarshalAs(UnmanagedType.Interface)] IDebugCodeContext2 pCodeContext);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int InsertTracepointAtCodeContext([In, MarshalAs(UnmanagedType.Interface)] IDebugCodeContext2 pCodeContext);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int DeleteBreakpointAtCodeContext([In, MarshalAs(UnmanagedType.Interface)] IDebugCodeContext2 pCodeContext);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int EnableBreakpointAtCodeContext([In, MarshalAs(UnmanagedType.Interface)] IDebugCodeContext2 pCodeContext, [In] bool enable);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int DoesCodeContextContainAnyBreakpoints([In, MarshalAs(UnmanagedType.Interface)] IDebugCodeContext2 pCodeContext, [In] BREAKPOINT_FILTER_TYPE filter, out int answer);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int AdviseEvents([In, MarshalAs(UnmanagedType.Interface)] IBreakpointEvents pHandler);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int UnadviseEvents([In, MarshalAs(UnmanagedType.Interface)] IBreakpointEvents pHandler);
        [DispId(0x60010008)]
        Array Breakpoints { [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UNKNOWN)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }
    }
}