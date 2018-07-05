using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using SkypeAutoRecorder.Extensions;

namespace SkypeAutoRecorder.Configuration
{
    public partial class Settings
    {
        private const string DATE_TIME_PLACEHOLDER = "{date-time}";
        private const string CONTACT_PLACEHOLDER = "{contact}";
        private const string DURATION_PLACEHOLDER = "{duration}";
        private const string DATE_TIME_FORMAT = "yyyy-MM-dd HH.mm.ss";
        private const string DURATION_FORMAT = @"hh\.mm\.ss";

        /// <summary>
        /// File name where application settings are stored.
        /// </summary>
        private static readonly string _settingsFileName;

        /// <summary>
        /// Available sound sample frequencies.
        /// </summary>
        private static readonly List<string> _availableSoundSampleFrequencies = new List<string> { "32", "44.1", "48" };

        /// <summary>
        /// Available sound bitrates.
        /// </summary>
        private static readonly List<string> _availableSoundBitrates = new List<string> { "192", "256", "320" };

        /// <summary>
        /// Default pattern for name of the recorded file.
        /// </summary>
        public const string DEFAULT_FILE_NAME = "{date-time} {contact}.mp3";

        /// <summary>
        /// Folder where application settings are stored.
        /// </summary>
        public static readonly string SettingsFolder;

        /// <summary>
        /// The application name.
        /// </summary>
        public const string APPLICATION_NAME = "SkypeAutoRecorder";

        /// <summary>
        /// Initializes the <see cref="Settings"/> class. Loads saved settings or creates new.
        /// </summary>
        static Settings()
        {
            SettingsFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SkypeAutoRecorder");
            _settingsFileName = Path.Combine(SettingsFolder, "Settings.xml");

            if (File.Exists(_settingsFileName))
            {
                // Load settings from XML.
                var serializer = new XmlSerializer(typeof(Settings));
                using (var reader = new StreamReader(_settingsFileName))
                {
                    try
                    {
                        Current = (Settings)serializer.Deserialize(reader);
                    }
                    catch (InvalidOperationException)
                    {
                        Current = new Settings();
                    }
                }
            }
            else
            {
                Current = new Settings();
                Save();
                IsFirstStart = true;
            }
        }

        /// <summary>
        /// Gets or sets the current settings.
        /// </summary>
        /// <value>
        /// The current settings.
        /// </value>
        public static Settings Current { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether recorder was started for the first time.
        /// </summary>
        /// <value>
        /// <c>true</c> if recorder was started for the first time; otherwise, <c>false</c>.
        /// </value>
        public static bool IsFirstStart { get; set; }

        /// <summary>
        /// Gets the name of the temp wav file.
        /// </summary>
        /// <returns></returns>
        public static string GetTempFileName(string i = null)
        {
            return Path.Combine(Path.GetTempPath(),
                "sar_" + DateTime.Now.ToString("HH_mm_ss") + (i == null ? string.Empty : "_" + i) + ".wav");
        }

        /// <summary>
        /// Creates the name of the file by replacing placeholders with actual data.
        /// </summary>
        /// <param name="rawFileName">Name of the file with placeholders.</param>
        /// <param name="contact">The contact name.</param>
        /// <param name="dateTime">The date time.</param>
        /// <param name="duration">The duration of recorded conversation.</param>
        /// <returns>The actual file name for settings.</returns>
        public static string RenderFileName(string rawFileName, string contact, DateTime dateTime, TimeSpan duration)
        {
            if (rawFileName == null)
            {
                return null;
            }

            var fileName = rawFileName;

            // Check if file name is missing. Add default one.
            if (string.IsNullOrEmpty(Path.GetFileName(fileName)))
            {
                fileName += DEFAULT_FILE_NAME;
            }

            // Add extension if its missing.
            fileName = Path.ChangeExtension(fileName, "mp3");

            // Replace placeholders.
            fileName = fileName.Replace(DATE_TIME_PLACEHOLDER, dateTime.ToString(DATE_TIME_FORMAT));
            fileName = fileName.Replace(DURATION_PLACEHOLDER, duration.ToString(DURATION_FORMAT));
            fileName = fileName.Replace(CONTACT_PLACEHOLDER, contact);
            return fileName.GetSafeFileName();
        }

        /// <summary>
        /// Saves current settings to file.
        /// </summary>
        public static void Save()
        {
            // Create directory for application settings if it doesn't exists.
            var path = Path.GetDirectoryName(_settingsFileName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            // Save settings to XML.
            var serializer = new XmlSerializer(typeof(Settings));
            using (var writer = new StreamWriter(_settingsFileName))
            {
                serializer.Serialize(writer, Current);
            }
        }

        /// <summary>
        /// Checks if string with contacts contains specified contact.
        /// </summary>
        /// <param name="contacts">The contacts separated with comma or semicolon.</param>
        /// <param name="contact">The contact to find.</param>
        /// <returns><c>true</c> if contact is present in string; otherwise, <c>false</c>.</returns>
        private static bool contactsContain(string contacts, string contact)
        {
            var contactsList = contacts.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
                                       .Select(c => c.Trim().ToLower());

            return contactsList.Contains(contact.ToLower());
        }
    }
}
