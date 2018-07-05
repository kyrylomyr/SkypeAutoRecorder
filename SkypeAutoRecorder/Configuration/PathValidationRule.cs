using System.Globalization;
using System.Windows.Controls;

namespace SkypeAutoRecorder.Configuration
{
    /// <summary>
    /// Validation rule for WPF text boxes that verifies entered path and file name are correct.
    /// </summary>
    internal class PathValidationRule : ValidationRule
    {
        /// <summary>
        /// Performs validation checks on a value.
        /// </summary>
        /// <param name="value">The value from the binding target to check.</param>
        /// <param name="cultureInfo">The culture to use in this rule.</param>
        /// <returns>
        /// A <see cref="T:System.Windows.Controls.ValidationResult"/> object.
        /// </returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return ValidationHelper.ValidatePath(value as string)
                       ? new ValidationResult(true, null)
                       : new ValidationResult(false, "File name in invalid");
        }
    }
}
