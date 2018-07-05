using System;

namespace SkypeAutoRecorder.Core.WinApi
{
    /// <summary>
    /// Flags for <c>SendMessageTimeout</c> Windows API function.
    /// </summary>
    [Flags]
    internal enum SendMessageTimeoutFlags : uint
    {
        Normal                 = 0x0000,
        Block                  = 0x0001,
        AbortIfHung            = 0x0002,
        NoTimeoutIfNothingHung = 0x0008,
        ErrorOnExit            = 0x0020
    }
}
