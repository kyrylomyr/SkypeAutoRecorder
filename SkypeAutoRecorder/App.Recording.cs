using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using SkypeAutoRecorder.Configuration;
using SkypeAutoRecorder.Core;
using SkypeAutoRecorder.Core.Sound;
using SkypeAutoRecorder.Helpers;
using MessageBox = System.Windows.MessageBox;

namespace SkypeAutoRecorder
{
    public partial class App
    {
        private const string RECORD_SAVE_ERROR = "Saving recorded file as \"{0}\" has failed. File was saved as \"{1}\" instead. Do you want to open folder with file?";
        
        private readonly SkypeConnector _connector = new SkypeConnector();

        private readonly object _locker = new object();

        /// <summary>
        /// Final resulting file name after recording and all sound processing steps.
        /// </summary>
        private string _recordFileName;

        private DateTime _startRecordDateTime;

        private void initSkypeConnector()
        {
            _connector.Connected += updateGuiConnected;
            _connector.Disconnected += updateGuiDisconnected;
            _connector.ConversationStarted += connectorOnConversationStarted;
            _connector.ConversationEnded += updateGuiConversationEnded;
            _connector.RecordingStarted += connectorOnRecordingStarted;
            _connector.RecordingStopped += connectorOnRecordingStopped;
            _connector.RecordingCanceled += updateGuiRecordingCanceled;

            _connector.Enable();
        }

        private void connectorOnConversationStarted(object sender, ConversationEventArgs eventArgs)
        {
            updateGuiConversationStarted();

            _recordFileName = Settings.Current.GetRawFileName(eventArgs.CallerName);
            startRecording();
        }

        private void startRecording()
        {
            if (!string.IsNullOrEmpty(_recordFileName))
                _connector.StartRecording(Settings.GetTempFileName("In"), Settings.GetTempFileName("Out"));
        }

        private void connectorOnRecordingStarted(object sender, RecordingEventArgs eventArgs)
        {
            updateGuiRecordingStarted();
            _startRecordDateTime = DateTime.Now;
        }

        private void connectorOnRecordingStopped(object sender, RecordingEventArgs eventArgs)
        {
            updateGuiRecordingStopped();
            convertRecordedFile(eventArgs.CallerName, eventArgs.CallInFileName, eventArgs.CallOutFileName);
        }

        private void convertRecordedFile(string callerName, string inFileName, string outFileName)
        {
            // Prepare data for sound processing in a separate thread.
            var fileNames = new ProcessingThreadData
                            {
                                TempInFileName = inFileName,
                                TempOutFileName = outFileName,
                                RecordRawFileName = _recordFileName,
                                CallerName = callerName,
                                StartRecordDateTime = _startRecordDateTime
                            };

            // Need to use Thread not from ThreadPool, because we want to run sound processing
            // even after application closes.
            new Thread(soundProcessing).Start(fileNames);
        }

        private void soundProcessing(object dataObject)
        {
            var data = (ProcessingThreadData)dataObject;

            // Wait while files are in use.
            while (FilesHelper.FileIsInUse(data.TempInFileName) || FilesHelper.FileIsInUse(data.TempOutFileName)) {}

            var joinedFileName = joinSoundChannels(data.TempInFileName, data.TempOutFileName);
            if (joinedFileName == null)
                return;

            File.Delete(data.TempInFileName);
            File.Delete(data.TempOutFileName);

            // Encode merged file to MP3.
            var duration = DateTime.Now - data.StartRecordDateTime;
            var recordFileName = Settings.RenderFileName(
                data.RecordRawFileName, data.CallerName, data.StartRecordDateTime, duration);
            if (!DirectoriesHelper.CreateDirectory(recordFileName) || !encodeMp3(joinedFileName, recordFileName))
            {
                // Encode to settings folder with default file name if unable encode to the desired file name.
                recordFileName = Path.Combine(Settings.SettingsFolder, Settings.RenderFileName(
                    Settings.DEFAULT_FILE_NAME, data.CallerName, data.StartRecordDateTime, duration));

                if (!encodeMp3(joinedFileName, recordFileName))
                {
                    // If encoding fails anyway then return WAV file to user.
                    recordFileName = Path.ChangeExtension(recordFileName, "wav");
                    File.Copy(joinedFileName, recordFileName, true);
                }

                // Report about error and ask about opening folder with resulting file.
                if (MessageBox.Show(string.Format(RECORD_SAVE_ERROR, data.RecordRawFileName, recordFileName),
                        "Saving error", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                    Process.Start(Settings.SettingsFolder);
            }

            updateLastRecordFileName(recordFileName);
            File.Delete(joinedFileName);
        }

        private string joinSoundChannels(string fileName1, string fileName2)
        {
            var joinedFileName = Settings.GetTempFileName();
            return SoundProcessor.JoinChannels(fileName1, fileName2, joinedFileName, Settings.Current.SeparateSoundChannels)
                       ? joinedFileName
                       : null;
        }

        private bool encodeMp3(string joinedFileName, string recordFileName)
        {
            return SoundProcessor.EncodeMp3(joinedFileName, recordFileName, Settings.Current.VolumeScale,
                                            Settings.Current.HighQualitySound, Settings.Current.SoundSampleFrequency,
                                            Settings.Current.SoundBitrate);
        }
    }
}