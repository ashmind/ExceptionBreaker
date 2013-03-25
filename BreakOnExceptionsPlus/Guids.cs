// Guids.cs
// MUST match guids.h

using System;

namespace BreakOnExceptionsPlus
{
    internal static class GuidList
    {
        public const string PackageString = "a83e8a33-e775-4a79-be41-efe20007eebd";
        public const string CommandSetString = "9d55da8f-2be1-44dc-a94a-08154f98f634";

        public static readonly Guid CommandSet = new Guid(CommandSetString);
        public static readonly Guid OutputPane = new Guid("fb57af1a-e1f6-4a87-97f6-0b1d4add8e24");
    };
}