namespace SkypeAutoRecorder.Core.SkypeApi
{
    /// <summary>
    /// Results of attaching to Skype.
    /// </summary>
    internal enum SkypeAttachResult : uint
    {
        AttachSuccess              = 0,
        AttachPendingAuthorization = 1,
        AttachRefused              = 2,
        AttachNotAvailable         = 3,
        AttachAvailable            = 0x8001
    }
}
