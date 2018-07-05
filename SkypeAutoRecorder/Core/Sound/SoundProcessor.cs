using System.Diagnostics;
using System.IO;

namespace SkypeAutoRecorder.Core.Sound
{
    /// <summary>
    /// Provides methods for processing sound files recorded by Skype.
    /// </summary>
    internal class SoundProcessor
    {
        private static readonly string _soxPath;
        private static readonly string _lamePath;

        /// <summary>
        /// Initializes the <see cref="SoundProcessor"/> class.
        /// </summary>
        static SoundProcessor()
        {
            var currentLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            
            // Get the path for Sox and Lame external applications used for sound processing.
            _soxPath = Path.Combine(currentLocation, @"Tools\Sox\sox.exe");
            _lamePath = Path.Combine(currentLocation, @"Tools\Lame\lame.exe");
        }

        /// <summary>
        /// Joins the two sound channels.
        /// </summary>
        /// <param name="channel1FileName">Name of the first channel file.</param>
        /// <param name="channel2FileName">Name of the second channel file.</param>
        /// <param name="resultFileName">Name for the resulting file.</param>
        /// <param name="separateChannels">Set to <c>true</c> to create file with separate sound channels;
        /// set to <c>false</c> if channels should be mixed into one.</param>
        /// <returns><c>true</c> if joining finished successfuly; otherwise, <c>false</c>.</returns>
        public static bool JoinChannels(
            string channel1FileName, string channel2FileName, string resultFileName, bool separateChannels)
        {
            var mode = separateChannels ? "-M" : "-m";
            var arguments = $@"{mode} ""{channel1FileName}"" ""{channel2FileName}"" ""{resultFileName}""";
            return runProcess(_soxPath, arguments);
        }

        /// <summary>
        /// Encodes sound to MP3.
        /// </summary>
        /// <param name="wavFileName">Name of the input WAV file.</param>
        /// <param name="mp3FileName">Name for the resulting MP3 file.</param>
        /// <param name="volumeScale">Volume scale of the resulting file.</param>
        /// <param name="highQuality">Should be created MP3 file with high quality or not.</param>
        /// <param name="sampleFrequency">Sound sample frequency for high-quality MP3.</param>
        /// <param name="bitrate">Sound bitrate for high-quality MP3.</param>
        /// <returns><c>true</c> if encoding finished successfuly; otherwise, <c>false</c>.</returns>
        public static bool EncodeMp3(string wavFileName, string mp3FileName, int volumeScale,
            bool highQuality, string sampleFrequency = null, string bitrate = null)
        {
            var mode = highQuality ? $"--resample {sampleFrequency} -b {bitrate}" : "-V0";
            var arguments = $@"{mode} --scale {volumeScale} ""{wavFileName}"" ""{mp3FileName}""";
            return runProcess(_lamePath, arguments);
        }

        /// <summary>
        /// Runs the process of external application.
        /// </summary>
        /// <param name="app">The external application file name.</param>
        /// <param name="arguments">The arguments for running.</param>
        /// <returns><c>true</c> if process finished successfuly; otherwise, <c>false</c>.</returns>
        private static bool runProcess(string app, string arguments)
        {
            using (var process = new Process())
            {
                var processStartInfo = new ProcessStartInfo
                                       {
                                           FileName = app,
                                           Arguments = arguments,
                                           CreateNoWindow = true,
                                           UseShellExecute = false
                                       };
                process.StartInfo = processStartInfo;
                process.Start();
                process.WaitForExit();
                return process.ExitCode == 0;
            }
        }
    }
}
