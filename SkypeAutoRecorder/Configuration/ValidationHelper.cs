using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SkypeAutoRecorder.Configuration
{
    /// <summary>
    /// Provides helper methods for inputs validation.
    /// </summary>
    internal static class ValidationHelper
    {
        /// <summary>
        /// Validates the path and file name.
        /// </summary>
        /// <param name="path">The path and file name.</param>
        /// <returns><c>true</c> if provided path is valid; otherwise, <c>false</c>.</returns>
        public static bool ValidatePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            try
            {
                new System.IO.FileInfo(path);
                return true;
            }
            catch (ArgumentException)
            {
            }
            catch (System.IO.PathTooLongException)
            {
            }
            catch (NotSupportedException)
            {
            }

            return false;
        }

        /// <summary>
        /// Validates that inputs of parent <see cref="DependencyObject"/> and child objects don't contain
        /// validation errors.
        /// </summary>
        /// <param name="parent">The parent <see cref="DependencyObject"/>.</param>
        /// <returns><c>true</c> if all inputs are valid; otherwise, <c>false</c>.</returns>
        public static bool InputsAreValid(DependencyObject parent)
        {
            // Check parent and all child object for validation errors.
            return !Validation.GetHasError(parent) &&
                   LogicalTreeHelper.GetChildren(parent).OfType<DependencyObject>().All(InputsAreValid);
        }
    }
}