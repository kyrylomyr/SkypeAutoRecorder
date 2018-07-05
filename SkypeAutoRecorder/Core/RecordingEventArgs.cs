using System;

namespace SkypeAutoRecorder.Core
{
    /// <summary>
    /// Recording event arguments which provide file names.
    /// </summary>
    internal class RecordingEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecordingEventArgs"/> class.
        /// </summary>
        /// <param name="callerName">Name of the caller.</param>
        /// <param name="callInFileName">The file name of input sound channel.</param>
        /// <param name="callOutFileName">The file name of output sound channel.</param>
        public RecordingEventArgs(string callerName, string callInFileName, string callOutFileName)
        {
            CallerName = callerName;
            CallInFileName = callInFileName;
            CallOutFileName = callOutFileName;
        }

        /// <summary>
        /// Gets the name of the caller.
        /// </summary>
        /// <value>
        /// The name of the caller.
        /// </value>
        public string CallerName { get; private set; }

        /// <summary>
        /// Gets the file name of input sound channel.
        /// </summary>
        /// <value>
        /// The file name of input sound channel.
        /// </value>
        public string CallInFileName { get; private set; }

        /// <summary>
        /// Gets the file name of output sound channel.
        /// </summary>
        /// <value>
        /// The file name of output sound channel.
        /// </value>
        public string CallOutFileName { get; private set; }
    }
}