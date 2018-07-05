using System;
using System.Runtime.InteropServices;

namespace SkypeAutoRecorder.Core.WinApi
{
    /// <summary>
    /// Wrapper for Windows API functions.
    /// </summary>
    internal static class WinApiWrapper
    {
        #region External functions

        [DllImport("user32.dll")]
        private static extern uint RegisterWindowMessage(string message);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessageTimeout(
            IntPtr windowHandle,
            uint message,
            IntPtr wParam,
            IntPtr lParam,
            SendMessageTimeoutFlags flags,
            uint timeout,
            out IntPtr result);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessageTimeout(
            IntPtr windowHandle,
            uint message,
            IntPtr wParam,
            ref CopyDataStruct lParam,
            SendMessageTimeoutFlags flags,
            uint timeout,
            out IntPtr result);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string className, string title);

        #endregion

        /// <summary>
        /// Registers the Windows API message in the system.
        /// </summary>
        /// <param name="message">The Windows API message name.</param>
        /// <returns>Identifier of the registered message.</returns>
        public static uint RegisterApiMessage(string message)
        {
            var id = RegisterWindowMessage(message);
            if (id == 0)
            {
                throw new WinApiException("RegisterWindowMessage", "Failed to register " + message);
            }

            return id;
        }

        /// <summary>
        /// Sends the Windows API message.
        /// </summary>
        /// <param name="receiverHandle">The receiver handle.</param>
        /// <param name="message">The message.</param>
        /// <param name="param">The parameter.</param>
        public static void SendMessage(IntPtr receiverHandle, uint message, IntPtr param)
        {
            IntPtr result;
            if (SendMessageTimeout(receiverHandle, message, param, IntPtr.Zero,
                                   SendMessageTimeoutFlags.Normal, 100, out result).ToInt32() == 0)
            {
                throw new WinApiException("SendMessageTimeout", string.Empty);
            }
        }

        /// <summary>
        /// Sends the Windows API message.
        /// </summary>
        /// <param name="receiverHandle">The receiver handle.</param>
        /// <param name="message">The message.</param>
        /// <param name="param">The parameter.</param>
        /// <param name="data">The data.</param>
        public static void SendMessage(IntPtr receiverHandle, uint message, IntPtr param, ref CopyDataStruct data)
        {
            IntPtr result;
            if (SendMessageTimeout(receiverHandle, message, param, ref data,
                                   SendMessageTimeoutFlags.Normal, 100, out result).ToInt32() == 0)
            {
                throw new WinApiException("SendMessageTimeout", string.Empty);
            }
        }

        /// <summary>
        /// Checks that window with specified class name exists.
        /// </summary>
        /// <param name="windowClassName">Name of the window class.</param>
        /// <returns><c>true</c> if window exists; otherwise, <c>false</c>.</returns>
        public static bool WindowExists(string windowClassName)
        {
            return FindWindow(windowClassName, null) != IntPtr.Zero;
        }
    }
}
