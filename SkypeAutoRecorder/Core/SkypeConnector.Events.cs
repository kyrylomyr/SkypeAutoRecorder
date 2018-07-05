using System;

namespace SkypeAutoRecorder.Core
{
    internal partial class SkypeConnector
    {
        /// <summary>
        /// Delegate of the connection events handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public delegate void ConnectionEventHandler(object sender, EventArgs e);

        /// <summary>
        /// Delegate of the conversation events handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SkypeAutoRecorder.Core.ConversationEventArgs"/> instance
        /// containing the event data.</param>
        public delegate void ConversationEventHandler(object sender, ConversationEventArgs e);

        /// <summary>
        /// Delegate of the recording events handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SkypeAutoRecorder.Core.RecordingEventArgs"/> instance
        /// containing the event data.</param>
        public delegate void RecordingEventHandler(object sender, RecordingEventArgs e);

        /// <summary>
        /// Occurs when application is successfuly connected to the Skype.
        /// </summary>
        public event ConnectionEventHandler Connected;

        /// <summary>
        /// Occurs when application disconnects from the Skype.
        /// </summary>
        public event ConnectionEventHandler Disconnected;

        /// <summary>
        /// Occurs when conversation starts.
        /// </summary>
        public event ConversationEventHandler ConversationStarted;

        /// <summary>
        /// Occurs when conversation ends.
        /// </summary>
        public event ConversationEventHandler ConversationEnded;

        /// <summary>
        /// Occurs when recording starts.
        /// </summary>
        public event RecordingEventHandler RecordingStarted;

        /// <summary>
        /// Occurs when recording stops.
        /// </summary>
        public event RecordingEventHandler RecordingStopped;

        /// <summary>
        /// Occurs when recording is canceled.
        /// </summary>
        public event RecordingEventHandler RecordingCanceled;

        private void invokeConnected()
        {
            var handler = Connected;
            handler?.Invoke(this, EventArgs.Empty);
        }

        private void invokeDisconnected()
        {
            var handler = Disconnected;
            handler?.Invoke(this, EventArgs.Empty);
        }

        private void invokeConversationStarted(ConversationEventArgs e)
        {
            var handler = ConversationStarted;
            handler?.Invoke(this, e);
        }

        private void invokeConversationEnded(ConversationEventArgs e)
        {
            var handler = ConversationEnded;
            handler?.Invoke(this, e);
        }

        private void invokeRecordingStarted(RecordingEventArgs e)
        {
            var handler = RecordingStarted;
            handler?.Invoke(this, e);
        }

        private void invokeRecordingStopped(RecordingEventArgs e)
        {
            var handler = RecordingStopped;
            handler?.Invoke(this, e);
        }

        private void invokeRecordingCanceled(RecordingEventArgs e)
        {
            var handler = RecordingCanceled;
            handler?.Invoke(this, e);
        }
    }
}
