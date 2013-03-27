// PkgCmdID.cs
// MUST match PkgCmdID.h

using System;
using System.ComponentModel.Design;

namespace ExceptionBreaker {
    internal static class CommandIDs {
        public static readonly CommandID BreakOn = new CommandID(GuidList.CommandSet, 0x100);
    };
}