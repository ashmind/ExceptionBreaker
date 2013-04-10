using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.OLE.Interop;

// ReSharper disable CheckNamespace
namespace Microsoft.VisualStudio.Debugger.Interop.Internal {
// ReSharper restore CheckNamespace
    [ComImport, Guid("6517701C-2475-4CC7-B23C-148E0900FEF2"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDataTipManager {
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int AdviseEvents([In, MarshalAs(UnmanagedType.Interface)] IDataTipEvents pHandler);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int UnadviseEvents([In, MarshalAs(UnmanagedType.Interface)] IDataTipEvents pHandler);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int ShowChildDataTip([In, ComAliasName("AD7InteropA.DEBUG_PROPERTY_INFO"), MarshalAs(UnmanagedType.LPArray)] DEBUG_PROPERTY_INFO[] parentExpression, [In] ulong owningHwnd, [In] int x, [In] int y, [In, ComAliasName("Microsoft.VisualStudio.Debugger.Interop.Internal.DataTipIdentity"), MarshalAs(UnmanagedType.LPArray)] DataTipIdentity[] identity);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int ShowFrameEnumToolTip([In, MarshalAs(UnmanagedType.Interface)] IEnumDebugFrameInfo2 pFrame, [In] ulong hwndOwner, [In, ComAliasName("OLE.POINT"), MarshalAs(UnmanagedType.LPArray)] POINT[] pptTopLeft, [In, ComAliasName("OLE.RECT"), MarshalAs(UnmanagedType.LPArray)] RECT[] pHotRect, [In] int bHoverInvoked);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int ShowThreadToolTip([In, MarshalAs(UnmanagedType.Interface)] IDebugThread2 pThread, [In] ulong hwndOwner, [In, ComAliasName("OLE.POINT"), MarshalAs(UnmanagedType.LPArray)] POINT[] pptTopLeft, [In, ComAliasName("OLE.RECT"), MarshalAs(UnmanagedType.LPArray)] RECT[] pHotRect, [In] int bHoverInvoked);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int IsThreadToolTipActive(out bool pActive);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int GetThreadToolTipFramesCount(out int pCount);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int GetThreadToolTipFrame([In] int row, [MarshalAs(UnmanagedType.BStr)] out string pFrameText, out bool pIsExpandable);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int CloseThreadToolTip();
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int CloseTip([In, ComAliasName("Microsoft.VisualStudio.Debugger.Interop.Internal.DataTipIdentity"), MarshalAs(UnmanagedType.LPArray)] DataTipIdentity[] identity);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int OpenTip([In, ComAliasName("Microsoft.VisualStudio.Debugger.Interop.Internal.DataTipIdentity"), MarshalAs(UnmanagedType.LPArray)] DataTipIdentity[] identiy, [In, MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)] Array expressions);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int RemoveExpression([In, ComAliasName("Microsoft.VisualStudio.Debugger.Interop.Internal.DataTipIdentity"), MarshalAs(UnmanagedType.LPArray)] DataTipIdentity[] identity, [In, MarshalAs(UnmanagedType.BStr)] string expression);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int AddExpression([In, ComAliasName("Microsoft.VisualStudio.Debugger.Interop.Internal.DataTipIdentity"), MarshalAs(UnmanagedType.LPArray)] DataTipIdentity[] identity, [In, MarshalAs(UnmanagedType.BStr)] string expression);
    }
}