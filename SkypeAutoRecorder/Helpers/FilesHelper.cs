using System.IO;

namespace SkypeAutoRecorder.Helpers
{
    /// <summary>
    /// Helper methods to work with files.
    /// </summary>
    internal class FilesHelper
    {
        /// <summary>
        /// Check if file is in use.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns><c>true</c> if file is in use and can't be read now; otherwise, <c>false</c>.</returns>
        public static bool FileIsInUse(string fileName)
        {
            try
            {
                if (!File.Exists(fileName))
                    return false;

                using (new FileStream(fileName, FileMode.Open))
                {
                }
                return false;
            }
            catch (IOException)
            {
                return true;
            }
        }
    }
}
