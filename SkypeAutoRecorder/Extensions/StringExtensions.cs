using System.IO;
using System.Linq;

namespace SkypeAutoRecorder.Extensions
{
    public static class StringExtensions
    {
        public static string GetSafeFileName(this string fileName, char replace = '_')
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            return new string(fileName.Select(x => invalidChars.Contains(x) ? replace : x).ToArray());
        }
    }
}
