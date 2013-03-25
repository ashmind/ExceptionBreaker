using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable CheckNamespace
// I could have referenced Microsoft.VisualStudio.Debugger.Interop.Internal.dll, but it would be somewhat annoying I suppose
// as it is in PrivateAssemblies. Please let me know if I am wrong.
namespace Microsoft.VisualStudio.Debugger.Interop.Internal {
// ReSharper restore CheckNamespace
    [ComImport, Guid("05538AD7-8C70-4905-A21A-D2E72DC16805"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDebuggerInternalEvents {
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int OnSessionCreate([In, MarshalAs(UnmanagedType.Interface)] IDebugSession2 pSession);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int OnSessionDestroy([In, MarshalAs(UnmanagedType.Interface)] IDebugSession2 pSession);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int OnProcessRegister([In, MarshalAs(UnmanagedType.Interface)] IDebugCoreServer2 pServer, [In, MarshalAs(UnmanagedType.Interface)] IDebugProcess2 pProcess);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int OnProcessDeregister([In, MarshalAs(UnmanagedType.Interface)] IDebugCoreServer2 pServer, [In, MarshalAs(UnmanagedType.Interface)] IDebugProcess2 pProcess);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int OnCurrentProcessChange();
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int OnCurrentProgramChange();
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int OnCurrentThreadChange();
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int OnCurrentFrameChange();
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int OnCurrentStatementChange();
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int OnModeChange([In] uint NewDebugMode);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int OnEnterRunMode();
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int OnProcessCreate([In, MarshalAs(UnmanagedType.Interface)] IDebugProcess2 pProcess);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int OnProcessDestroy([In, MarshalAs(UnmanagedType.Interface)] IDebugProcess2 pProcess);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int OnThreadCreate([In, MarshalAs(UnmanagedType.Interface)] IDebugThread2 pThread);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int OnThreadDestroy([In, MarshalAs(UnmanagedType.Interface)] IDebugThread2 pThread);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int OnTimeContextChange();
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int OnShellModeChange([In] uint NewShellMode);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int OnSetNextStatement();
    }
}
