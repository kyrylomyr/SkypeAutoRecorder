using System;

namespace SkypeAutoRecorder
{
    /// <summary>
    /// Data to pass to another thread.
    /// </summary>
    internal class ProcessingThreadData
    {
        public string TempInFileName;
        public string TempOutFileName;
        public string RecordRawFileName;
        public string CallerName;
        public DateTime StartRecordDateTime;
    }
}