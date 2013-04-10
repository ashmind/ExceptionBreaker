// ReSharper disable CheckNamespace
namespace Microsoft.VisualStudio.Debugger.Interop.Internal {
// ReSharper restore CheckNamespace
    public enum IMAGEINFO_TYPE {
        IMAGEINFO_ANYCPU_PLATFORM = 0x10,
        IMAGEINFO_CONSOLE_APPLICATION = 1,
        IMAGEINFO_INTEROP_APPLICATION = 8,
        IMAGEINFO_MANAGED_APPLICATION = 4,
        IMAGEINFO_NATIVE_APPLICATION = 2,
        IMAGEINFO_X64_PLATFORM = 0x40,
        IMAGEINFO_X86_PLATFORM = 0x20
    }
}