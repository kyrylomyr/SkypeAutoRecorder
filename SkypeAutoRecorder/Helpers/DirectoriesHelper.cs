using System.IO;

namespace SkypeAutoRecorder.Helpers
{
    internal class DirectoriesHelper
    {
        /// <summary>
        /// Creates the directory if it doesn't exists.
        /// </summary>
        /// <param name="fileName">Full name of the file whose directory need to create.</param>
        /// <returns><c>true</c> if directory is already exists or was created successfully;
        /// otherwise, <c>false</c>.</returns>
        public static bool CreateDirectory(string fileName)
        {
            var dir = Path.GetDirectoryName(fileName);
            var dirNotFound = false;
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                try
                {
                    Directory.CreateDirectory(dir);
                }
                catch (DirectoryNotFoundException)
                {
                    // This exception is thrown when target dir is not available (for example, some removable drive).
                    dirNotFound = true;
                }
            }

            return !dirNotFound;
        }
    }
}
