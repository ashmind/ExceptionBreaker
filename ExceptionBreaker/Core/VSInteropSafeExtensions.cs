using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;

// This might be a microoptimization -- it is possible to achieve similar results with delegates.
// However since VS is often slow enough already, I feel optimizations matter.

namespace ExceptionBreaker.Core {
    public static class VSInteropExtensions {
		private static class EmptyCache<T> {
			public static readonly T[] Array = new T[0];
		}
        // Out parameter => return value
        public static IDebugBreakpointResolution2 GetBreakpointResolutionSafe(this IDebugBoundBreakpoint2 @object) {
			IDebugBreakpointResolution2 result;
			var hr = @object.GetBreakpointResolution(out result);
            if (hr != VSConstants.S_OK)
                Marshal.ThrowExceptionForHR(hr);

            return result;
        } 
        public static IDebugDocumentContext2 GetDocumentContextSafe(this IDebugCodeContext2 @object) {
			IDebugDocumentContext2 result;
			var hr = @object.GetDocumentContext(out result);
            if (hr != VSConstants.S_OK)
                Marshal.ThrowExceptionForHR(hr);

            return result;
        } 
        public static String GetNameSafe(this IDebugDocumentContext2 @object, UInt32 gnType) {
			String result;
			var hr = @object.GetName(gnType, out result);
            if (hr != VSConstants.S_OK)
                Marshal.ThrowExceptionForHR(hr);

            return result;
        } 

        // Array parameter => return value
        public static BP_RESOLUTION_INFO GetResolutionInfoSafe(this IDebugBreakpointResolution2 @object, UInt32 dwFields) {
			var result = new BP_RESOLUTION_INFO[1];
			var hr = @object.GetResolutionInfo(dwFields, result);
            if (hr != VSConstants.S_OK)
                Marshal.ThrowExceptionForHR(hr);

            return result[0];
        } 

        // COM enum out parameter => return array
        public static IDebugBoundBreakpoint2[] GetBreakpointsAsArraySafe(this IDebugBreakpointEvent2 @object) {
			IEnumDebugBoundBreakpoints2 @enum;
			var hr = @object.EnumBreakpoints(out @enum);
			if (hr == VSConstants.S_FALSE)
                return EmptyCache<IDebugBoundBreakpoint2>.Array;

            if (hr != VSConstants.S_OK)
                Marshal.ThrowExceptionForHR(hr);

            return @enum.ToArraySafe();
        } 
        public static EXCEPTION_INFO[] GetSetExceptionsAsArraySafe(this IDebugSession2 @object, IDebugProgram2 pProgram, String pszProgram, ref Guid guidType) {
			IEnumDebugExceptionInfo2 @enum;
			var hr = @object.EnumSetExceptions(pProgram, pszProgram, ref guidType, out @enum);
			if (hr == VSConstants.S_FALSE)
                return EmptyCache<EXCEPTION_INFO>.Array;

            if (hr != VSConstants.S_OK)
                Marshal.ThrowExceptionForHR(hr);

            return @enum.ToArraySafe();
        } 
        public static EXCEPTION_INFO[] GetDefaultExceptionsAsArraySafe(this IDebugSession2 @object, EXCEPTION_INFO[] pParentException) {
			IEnumDebugExceptionInfo2 @enum;
			var hr = @object.EnumDefaultExceptions(pParentException, out @enum);
			if (hr == VSConstants.S_FALSE)
                return EmptyCache<EXCEPTION_INFO>.Array;

            if (hr != VSConstants.S_OK)
                Marshal.ThrowExceptionForHR(hr);

            return @enum.ToArraySafe();
        } 

        // COM enum => array
        public static IDebugBoundBreakpoint2[] ToArraySafe(this IEnumDebugBoundBreakpoints2 @enum) {
            uint count;
            var hr = @enum.GetCount(out count);
            if (hr != VSConstants.S_OK)
                Marshal.ThrowExceptionForHR(hr);

            if (count == 0)
                return EmptyCache<IDebugBoundBreakpoint2>.Array;

            var buffer = new IDebugBoundBreakpoint2[count];
            var countFetched = 0U;
            hr = @enum.Next(count, buffer, ref countFetched);
            if (hr != VSConstants.S_OK)
                Marshal.ThrowExceptionForHR(hr);

            return buffer;
        } 
        public static EXCEPTION_INFO[] ToArraySafe(this IEnumDebugExceptionInfo2 @enum) {
            uint count;
            var hr = @enum.GetCount(out count);
            if (hr != VSConstants.S_OK)
                Marshal.ThrowExceptionForHR(hr);

            if (count == 0)
                return EmptyCache<EXCEPTION_INFO>.Array;

            var buffer = new EXCEPTION_INFO[count];
            var countFetched = 0U;
            hr = @enum.Next(count, buffer, ref countFetched);
            if (hr != VSConstants.S_OK)
                Marshal.ThrowExceptionForHR(hr);

            return buffer;
        } 
    }
}
