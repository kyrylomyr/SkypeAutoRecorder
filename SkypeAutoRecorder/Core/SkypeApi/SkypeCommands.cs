namespace SkypeAutoRecorder.Core.SkypeApi
{
    /// <summary>
    /// Commands to control Skype when connection is already established.
    /// </summary>
    internal static class SkypeCommands
    {
        public const string START_RECORD_OUTPUT = "ALTER CALL {0} SET_OUTPUT SOUNDCARD=\"default\", FILE=\"{1}\"";
        public const string START_RECORD_INPUT  = "ALTER CALL {0} SET_CAPTURE_MIC FILE=\"{1}\"";
        public const string END_RECORD_OUTPUT   = "ALTER CALL {0} SET_OUTPUT SOUNDCARD=\"default\", FILE=\"\"";
        public const string END_RECORD_INPUT    = "ALTER CALL {0} SET_CAPTURE_MIC FILE=\"\"";
        public const string GET_CALLER_NAME     = "GET CALL {0} PARTNER_HANDLE";
        public const string PING                = "PING";
    }
}
