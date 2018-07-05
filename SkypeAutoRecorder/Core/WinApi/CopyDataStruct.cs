using System.Runtime.InteropServices;

namespace SkypeAutoRecorder.Core.WinApi
{
    /// <summary>
    /// Contains data sent by Skype in messages to the handler.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct CopyDataStruct
    {
        public string Id;
        public int Size;
        public string Data;
    }
}
