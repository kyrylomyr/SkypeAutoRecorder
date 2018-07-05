using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace SkypeAutoRecorder.Helpers
{
    /// <summary>
    /// Manages global system-wide hot keys.
    /// </summary>
    public sealed class GlobalHotKey : IDisposable
    {
        #region Windows API interraction

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(
            IntPtr windowHandle, int hotketId, uint keysModifiers, uint virtualKey);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr windowHandle, int hotketId);

        private const int HotkeyMessage = 0x0312;

        #endregion

        /// <summary>
        /// Receives Windows API messages.
        /// </summary>
        private readonly HwndSource _windowHandleSource;

        public GlobalHotKey()
        {
            _windowHandleSource = new HwndSource(new HwndSourceParameters());
            _windowHandleSource.AddHook(messagesHook);
        }

        /// <summary>
        /// Hooks Windows messages to the window.
        /// </summary>
        /// <param name="windowHandle">The window handle.</param>
        /// <param name="message">The message ID.</param>
        /// <param name="wParam">The message's wParam value.</param>
        /// <param name="lParam">The message's lParam value.</param>
        /// <param name="handled">If set to <c>true</c> then message was handled; otherwise, <c>false</c>.</param>
        /// <returns></returns>
        private IntPtr messagesHook(
            IntPtr windowHandle, int message, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (message == HotkeyMessage)
            {
                
            }
        }

        /// <summary>
        /// Unregisters hotkeys.
        /// </summary>
        public void Dispose()
        {
            _windowHandleSource.RemoveHook(messagesHook);
        }
    }
}
