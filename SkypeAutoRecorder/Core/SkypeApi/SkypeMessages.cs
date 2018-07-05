namespace SkypeAutoRecorder.Core.SkypeApi
{
    /// <summary>
    /// Messages which Skype sends to attached handlers.
    /// </summary>
    internal static class SkypeMessages
    {
        public const string CONNECTION_STATUS_ONLINE  = "CONNSTATUS ONLINE";
        public const string CONNECTION_STATUS_OFFLINE = "CONNSTATUS OFFLINE";
        public const string PONG                      = "PONG";
    }
}
