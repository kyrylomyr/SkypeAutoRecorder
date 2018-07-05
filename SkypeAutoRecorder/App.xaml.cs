using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using GlobalHotKey;
using SkypeAutoRecorder.Configuration;
using SkypeAutoRecorder.Core;
using SkypeAutoRecorder.Helpers;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace SkypeAutoRecorder
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private readonly UniqueInstanceChecker _instanceChecker =
            new UniqueInstanceChecker("SkypeAutoRecorderOneInstanceMutex");

        // Icons from the resources for displaying application status.
        private readonly Icon _disconnectedIcon = new Icon(Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("SkypeAutoRecorder.Images.DisconnectedTrayIcon.ico"));
        private readonly Icon _connectedIcon = new Icon(Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("SkypeAutoRecorder.Images.ConnectedTrayIcon.ico"));
        private readonly Icon _recordingIcon = new Icon(Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("SkypeAutoRecorder.Images.RecordingTrayIcon.ico"));

        private NotifyIcon _trayIcon;
        private MenuItem _startRecordingMenuItem;
        private MenuItem _cancelRecordingMenuItem;
        private MenuItem _browseDefaultMenuItem;
        private MenuItem _browseLastRecordMenuItem;

        private HotKeyManager _hotKeyManager;
        private HotKey _startRecordingHotKey;
        private HotKey _cancelRecordingHotKey;

        private string _lastRecordFileName;

        private SettingsWindow _settingsWindow;
        private AboutWindow _aboutWindow;

        private void appStartup(object sender, StartupEventArgs e)
        {
            // Only one instance of SkypeAutoRecorder is allowed.
            if (_instanceChecker.IsAlreadyRunning())
            {
                System.Windows.MessageBox.Show("SkypeAutoRecorder is already running.", "SkypeAutoRecorder",
                                               MessageBoxButton.OK, MessageBoxImage.Exclamation);
                Shutdown();
                return;
            }

            buildTrayIcon();
            createHotKeyManager();
            initSkypeConnector();

            if (Settings.IsFirstStart)
            {
                System.Windows.MessageBox.Show(
                    "Thank you for choosing SkypeAutoRecorder!\r\nPlease, make sure that all call members are aware of recording!",
                    "SkypeAutoRecorder",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void buildTrayIcon()
        {
            _trayIcon = new NotifyIcon
            {
                ContextMenu = new ContextMenu(),
                Visible = true
            };

            setTrayIconWaitingSkype();

            // Add context menu.
            _startRecordingMenuItem = new MenuItem("Start recording", (sender, args) => startRecordingMenuItemClick())
                                      {
                                          Shortcut = Shortcut.CtrlShiftF5, Enabled = false, DefaultItem = true
                                      };
            _trayIcon.ContextMenu.MenuItems.Add(_startRecordingMenuItem);

            _cancelRecordingMenuItem = new MenuItem("Cancel recording", (sender, args) => cancelRecordingMenuItemClick())
                                       {
                                           Shortcut = Shortcut.CtrlShiftF10, Enabled = false
                                       };
            _trayIcon.ContextMenu.MenuItems.Add(_cancelRecordingMenuItem);

            _trayIcon.ContextMenu.MenuItems.Add("-");

            _browseDefaultMenuItem = new MenuItem("Browse records", (sender, args) => openRecordsDefaultFolder());
            updateBrowseDefaultMenuItem();
            _trayIcon.ContextMenu.MenuItems.Add(_browseDefaultMenuItem);

            _browseLastRecordMenuItem =
                new MenuItem("Browse last record", (sender, args) => openLastRecordFolder()) { Enabled = false };
            _trayIcon.ContextMenu.MenuItems.Add(_browseLastRecordMenuItem);

            _trayIcon.ContextMenu.MenuItems.Add("-");
            _trayIcon.ContextMenu.MenuItems.Add("Settings", (sender, args) => openSettingsWindow());
            _trayIcon.ContextMenu.MenuItems.Add("-");
            _trayIcon.ContextMenu.MenuItems.Add("Help", openHelp);
            _trayIcon.ContextMenu.MenuItems.Add("About", openAboutWindow);
            _trayIcon.ContextMenu.MenuItems.Add("-");
            _trayIcon.ContextMenu.MenuItems.Add("Close", (sender, e) => Shutdown());

            _trayIcon.MouseDoubleClick += trayIconOnMouseDoubleClick;
        }

        private void createHotKeyManager()
        {
            _hotKeyManager = new HotKeyManager();
            _hotKeyManager.KeyPressed += onHotKeyPressed;
            _startRecordingHotKey = _hotKeyManager.Register(Key.F5, ModifierKeys.Control | ModifierKeys.Shift);
            _cancelRecordingHotKey = _hotKeyManager.Register(Key.F10, ModifierKeys.Control | ModifierKeys.Shift);
        }

        private void updateGuiConnected(object sender, EventArgs eventArgs)
        {
            setTrayIconReady();
            updateStartCancelRecordMenuItems(false, false);
        }

        private void updateGuiDisconnected(object sender, EventArgs eventArgs)
        {
            setTrayIconWaitingSkype();
            updateStartCancelRecordMenuItems(false, false);
        }

        private void updateGuiConversationStarted()
        {
            updateStartCancelRecordMenuItems(true, false);
        }

        private void updateGuiConversationEnded(object sender, ConversationEventArgs eventArgs)
        {
            updateStartCancelRecordMenuItems(false, false);
        }

        private void updateGuiRecordingStarted()
        {
            setTrayIconRecording();
            updateStartCancelRecordMenuItems(false, true);
        }

        private void updateGuiRecordingStopped()
        {
            setTrayIconReady();
            updateStartCancelRecordMenuItems(true, false);
        }

        private void updateGuiRecordingCanceled(object sender, RecordingEventArgs eventArgs)
        {
            setTrayIconReady();
            updateStartCancelRecordMenuItems(false, false);
        }

        private void updateStartCancelRecordMenuItems(bool enableStart, bool enableCancel)
        {
            _startRecordingMenuItem.Enabled = enableStart;
            _cancelRecordingMenuItem.Enabled = enableCancel;
        }

        private void setTrayIconWaitingSkype()
        {
            _trayIcon.Icon = _disconnectedIcon;
            _trayIcon.Text = Settings.APPLICATION_NAME + ": Waiting for Skype";
        }

        private void setTrayIconReady()
        {
            _trayIcon.Icon = _connectedIcon;
            _trayIcon.Text = Settings.APPLICATION_NAME + ": Ready";
        }

        private void setTrayIconRecording()
        {
            _trayIcon.Icon = _recordingIcon;
            _trayIcon.Text = Settings.APPLICATION_NAME + ": Recording";
        }

        private void trayIconOnMouseDoubleClick(object sender, MouseEventArgs mouseEventArgs)
        {
            if (_startRecordingMenuItem.DefaultItem)
                startRecordingMenuItemClick();
            else if (_cancelRecordingMenuItem.DefaultItem)
                cancelRecordingMenuItemClick();
        }

        private void updateBrowseDefaultMenuItem()
        {
            _browseDefaultMenuItem.Enabled = !string.IsNullOrEmpty(Settings.Current.DefaultRawFileName);
        }

        private void updateLastRecordFileName(string fileName)
        {
            lock (_locker)
            {
                _lastRecordFileName = fileName;
                _browseLastRecordMenuItem.Enabled = !string.IsNullOrEmpty(_lastRecordFileName);
            }
        }

        private void onHotKeyPressed(object sender, KeyPressedEventArgs keyPressedEventArgs)
        {
            if (keyPressedEventArgs.HotKey.Equals(_startRecordingHotKey))
                startRecordingMenuItemClick();
            else if (keyPressedEventArgs.HotKey.Equals(_cancelRecordingHotKey))
                cancelRecordingMenuItemClick();
        }

        private void startRecordingMenuItemClick()
        {
            if (!_startRecordingMenuItem.Enabled || !_connector.ConversationIsActive || _connector.IsRecording)
                return;
            
            _recordFileName = Settings.Current.DefaultRawFileName;
            startRecording();
        }

        private void cancelRecordingMenuItemClick()
        {
            if (_cancelRecordingMenuItem.Enabled)
            {
                var result = System.Windows.MessageBox.Show(
                    "The recording will be canceled and already recorded conversation lost. Do you really want to proceed?",
                    "SkypeAutoRecorder",
                    MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

                if (result == MessageBoxResult.Yes)
                    _connector.CancelRecording();
            }
        }

        private void openRecordsDefaultFolder()
        {
            var path = Settings.Current.DefaultRawFileName;

            // Clear default records path from all placeholders. Need to remove chars starting from the first
            // placeholder and then fix it by removing all chars after last backslash.
            var i = path.IndexOf('{');
            if (i >= 0)
                path = path.Remove(i);

            i = path.LastIndexOf('\\');
            if (i >= 0)
                path = path.Remove(i);

            // Try to open resulting path without placeholders.
            // If it's incorrect or doesn't exist, Explorer opens some default folder automatically.
            if (!string.IsNullOrEmpty(path))
                Process.Start("explorer.exe", path);
        }

        private void openLastRecordFolder()
        {
            lock (_locker)
            {
                var args = File.Exists(_lastRecordFileName)
                               ? $"/select,\"{_lastRecordFileName}\""
                               : "\"" + Path.GetDirectoryName(_lastRecordFileName) + "\"";
                Process.Start("explorer.exe", args);
            }
        }

        private void openSettingsWindow()
        {
            if (_settingsWindow != null && _settingsWindow.IsLoaded)
                return;

            // Create copy of the current settings to have a possibility of rollback changes.
            var settingsCopy = (Settings)Settings.Current.Clone();

            // Create settings window with copied settings.
            _settingsWindow = new SettingsWindow(settingsCopy);
            _settingsWindow.Closed += settingsWindowOnClosed;
            _settingsWindow.ShowDialog();
        }

        private void settingsWindowOnClosed(object sender, EventArgs eventArgs)
        {
            if (_settingsWindow.DialogResult == true)
            {
                // Replace current settings and save them to file if user has accepted changes in settings window.
                Settings.Current = _settingsWindow.NewSettings;
                Settings.Save();

                updateBrowseDefaultMenuItem();
            }
        }

        private void openAboutWindow(object sender, EventArgs eventArgs)
        {
            if (_aboutWindow != null && _aboutWindow.IsLoaded)
                return;

            _aboutWindow = new AboutWindow();
            _aboutWindow.ShowDialog();
        }

        private void openHelp(object sender, EventArgs eventArgs)
        {
            Process.Start("http://skypeautorecorder.codeplex.com/documentation");
        }

        private void onApplicationExit(object sender, ExitEventArgs e)
        {
            _connector.CancelRecording();

            _trayIcon?.Dispose();
            _connector?.Dispose();

            _instanceChecker.Release();
        }
    }
}