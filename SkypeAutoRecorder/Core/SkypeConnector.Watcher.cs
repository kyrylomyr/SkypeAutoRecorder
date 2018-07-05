using System;
using System.Timers;
using SkypeAutoRecorder.Core.SkypeApi;
using SkypeAutoRecorder.Core.WinApi;

namespace SkypeAutoRecorder.Core
{
    internal partial class SkypeConnector
    {
        /// <summary>
        /// Class name of the Skype window that actually allows attaching and send event messages to applications.
        /// </summary>
        private const string SKYPE_MAIN_WINDOW_CLASS = "tSkMainForm";
        
        /// <summary>
        /// Login window of the Skype that doesn't provide API.
        /// </summary>
        private const string SKYPE_LOGIN_WINDOW_CLASS = "TLoginForm";

        /// <summary>
        /// Periodicity of checking that Skype is still working (in ms).
        /// </summary>
        private const double WATCH_INTERVAL = 1000;

        /// <summary>
        /// Last time when PONG was recieved from Skype.
        /// </summary>
        private DateTime _lastPong;

        private readonly Timer _skypeWatcher;

        private void skypeWatcherHandler(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            // Check that Skype window that provides API is active now.
            var skypeIsActive = WinApiWrapper.WindowExists(SKYPE_MAIN_WINDOW_CLASS) &&
                                !WinApiWrapper.WindowExists(SKYPE_LOGIN_WINDOW_CLASS);

            // Ping-pong with Skype to check if we still are subscribed to its messages.
            if (IsConnected && skypeIsActive)
            {
                sendSkypeCommand(SkypeCommands.PING);
                
                // Check when last PONG was recieved. Let 3 lost PONGs are OK.
                var diff = DateTime.Now - _lastPong;
                if (diff.Milliseconds > WATCH_INTERVAL * 3)
                {
                    disconnect();
                    return;
                }
            }

            if (!IsConnected && skypeIsActive)
            {
                enableSkypeMessaging();
            }
            else if (IsConnected && !skypeIsActive)
            {
                disconnect();
            }
        }

        private void enableSkypeMessaging()
        {
            // Register API messages for communicating with Skype.
            _skypeApiDiscover = WinApiWrapper.RegisterApiMessage(SkypeControlApiMessages.DISCOVER);
            _skypeApiAttach = WinApiWrapper.RegisterApiMessage(SkypeControlApiMessages.ATTACH);

            // Register Skype messages handler if Skype is active.
            WinApiWrapper.SendMessage(_broadcastHandle, _skypeApiDiscover, _windowHandleSource.Handle);
        }
    }
}
