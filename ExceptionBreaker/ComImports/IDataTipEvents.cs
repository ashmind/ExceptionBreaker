using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable CheckNamespace
namespace Microsoft.VisualStudio.Debugger.Interop.Internal {
// ReSharper restore CheckNamespace
    [ComImport, Guid("C7097399-F1BF-4C37-ABEC-44A787170A46"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDataTipEvents {
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int TearOff([In, ComAliasName("Microsoft.VisualStudio.Debugger.Interop.Internal.DataTipIdentity"), MarshalAs(UnmanagedType.LPArray)] DataTipIdentity[] Id, [In, ComAliasName("AD7InteropA.DEBUG_PROPERTY_INFO")] DEBUG_PROPERTY_INFO info, [In] int screenX, [In] int screenY, [In] int width, [In] int height);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int UndoTearOff([In, ComAliasName("Microsoft.VisualStudio.Debugger.Interop.Internal.DataTipIdentity"), MarshalAs(UnmanagedType.LPArray)] DataTipIdentity[] Id, [In, ComAliasName("AD7InteropA.DEBUG_PROPERTY_INFO")] DEBUG_PROPERTY_INFO info);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int Closing([In, ComAliasName("Microsoft.VisualStudio.Debugger.Interop.Internal.DataTipIdentity"), MarshalAs(UnmanagedType.LPArray)] DataTipIdentity[] Id, [In, ComAliasName("AD7InteropA.DEBUG_PROPERTY_INFO")] DEBUG_PROPERTY_INFO info);
    }
}