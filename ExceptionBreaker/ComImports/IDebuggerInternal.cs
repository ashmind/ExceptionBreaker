using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Debugger.Interop;

// ReSharper disable CheckNamespace
namespace Microsoft.VisualStudio.Debugger.Interop.Internal {
// ReSharper restore CheckNamespace
    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("1DA40549-8CCC-48CF-B99B-FC22FE3AFEDF")]
    public interface IDebuggerInternal {
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int GetSourceFileWithChecksum([In, MarshalAs(UnmanagedType.BStr)] string bstrSearchFilePath, [In] ref Guid checksumAlgorithm, [In, MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)] Array Checksum, [MarshalAs(UnmanagedType.BStr)] out string bstrFoundFilePath);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int GetSourceFileFromDocumentContext([In, MarshalAs(UnmanagedType.Interface)] IDebugDocumentContext2 pDocumentContext, [In, MarshalAs(UnmanagedType.Interface)] IDebugCodeContext2 pCodeContext, [MarshalAs(UnmanagedType.BStr)] out string bstrFoundFilePath, out Guid checksumAlgorithm, [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)] out Array Checksum);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int GetCodeContextOfExpression([In, MarshalAs(UnmanagedType.BStr)] string expression, [MarshalAs(UnmanagedType.Interface)] out IDebugCodeContext2 ppCodeContext, [MarshalAs(UnmanagedType.BStr)] out string error);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int GetDebuggerOption([In] DEBUGGER_OPTIONS option, out uint value);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int GetDebuggerStringOption([In] DEBUGGER_STRING_OPTIONS option, [MarshalAs(UnmanagedType.BStr)] out string value);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int SetDebuggerOption([In] DEBUGGER_OPTIONS option, [In] uint value);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int SetDebuggerStringOption([In] DEBUGGER_STRING_OPTIONS option, [In, MarshalAs(UnmanagedType.BStr)] string value);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int SetNextStatement([In, MarshalAs(UnmanagedType.Interface)] IDebugCodeContext2 pCodeContext);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int RunToStatement([In, MarshalAs(UnmanagedType.Interface)] IDebugCodeContext2 pCodeContext);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int GoToSource([In, MarshalAs(UnmanagedType.Interface)] IDebugDocumentContext2 pDocContext, [In, MarshalAs(UnmanagedType.Interface)] IDebugCodeContext2 pCodeContext);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int GoToDisassembly([In, MarshalAs(UnmanagedType.Interface)] IDebugCodeContext2 pCodeContext, [In, MarshalAs(UnmanagedType.Interface)] IDebugProgram2 pProgramOfCodeContext);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int EditBreakpoint([In, MarshalAs(UnmanagedType.Interface)] IBreakpoint pBreakpoint, [In] BREAKPOINT_EDIT_OPERATION op);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int SetEngineMetric([In] ref Guid engine, [In, MarshalAs(UnmanagedType.BStr)] string metric, [In, MarshalAs(UnmanagedType.Struct)] object var);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int UpdateAddressMarkers();
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int DeleteAddressMarkers();
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int ShowVisualizer([In, ComAliasName("AD7InteropA.DEBUG_PROPERTY_INFO"), MarshalAs(UnmanagedType.LPArray)] DEBUG_PROPERTY_INFO[] data, [In] uint visualizerId);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int UpdateExpressionValue([In, ComAliasName("AD7InteropA.DEBUG_PROPERTY_INFO"), MarshalAs(UnmanagedType.LPArray)] DEBUG_PROPERTY_INFO[] data, [In, MarshalAs(UnmanagedType.BStr)] string newValue, [MarshalAs(UnmanagedType.BStr)] out string error);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int CreateObjectID([In, ComAliasName("AD7InteropA.DEBUG_PROPERTY_INFO"), MarshalAs(UnmanagedType.LPArray)] DEBUG_PROPERTY_INFO[] data);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int DestroyObjectID([In, ComAliasName("AD7InteropA.DEBUG_PROPERTY_INFO"), MarshalAs(UnmanagedType.LPArray)] DEBUG_PROPERTY_INFO[] data);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int IndicateEvalRequiresRefresh([In, ComAliasName("AD7InteropA.DEBUG_PROPERTY_INFO"), MarshalAs(UnmanagedType.LPArray)] DEBUG_PROPERTY_INFO[] src);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int RegisterInternalEventSink([In, MarshalAs(UnmanagedType.Interface)] IDebuggerInternalEvents pEvents);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int UnregisterInternalEventSink([In, MarshalAs(UnmanagedType.Interface)] IDebuggerInternalEvents pEvents);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int IsProcessActivelyBeingDebugged([In, MarshalAs(UnmanagedType.Interface)] IDebugProcess2 pProcess, out bool retVal);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int OnCaretMoved();
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int GetSymbolPathInternal([MarshalAs(UnmanagedType.BStr)] out string pbstrSymbolPath, [MarshalAs(UnmanagedType.BStr)] out string pbstrSymbolCachePath);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int GetImageInfo([In, MarshalAs(UnmanagedType.BStr)] string imageName, [Out, ComAliasName("Microsoft.VisualStudio.Debugger.Interop.Internal.IMAGEINFO_TYPE"), MarshalAs(UnmanagedType.LPArray)] IMAGEINFO_TYPE[] pImageInfoType);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int IsInteropSupported([In, MarshalAs(UnmanagedType.BStr)] string imageName);
        [DispId(0x6001001b)]
        IDebugSession3 CurrentSession { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }
        [DispId(0x6001001c)]
        IDebugProgram2 CurrentProgram { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [param: In, MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] set; }
        [DispId(0x6001001e)]
        IDebugThread2 CurrentThread { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [param: In, MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] set; }
        [DispId(0x60010020)]
        IEnumDebugFrameInfo2 CurrentStack { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }
        [DispId(0x60010021)]
        IDebugStackFrame2 CurrentStackFrame { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [param: In, MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] set; }
        [DispId(0x60010023)]
        IDebugCodeContext2 CurrentCodeContext { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }
        [DispId(0x60010024)]
        IDebugStackFrame2 TopMostStackFrame { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }
        [DispId(0x60010025)]
        IDebugCodeContext2 TopMostCodeContext { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }
        [DispId(0x60010026)]
        bool InDisassemblyMode { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] set; }
        [DispId(0x60010028)]
        bool InApplyCodeChanges { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }
        [DispId(0x60010029)]
        bool InBreakMode { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }
        [DispId(0x6001002a)]
        IBreakpointManager BreakpointManager { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }
        [DispId(0x6001002b)]
        bool ArePendingEditsBlockingSetNext { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }
        [DispId(0x6001002c)]
        IDataTipManager DataTipManager { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }
    }
}