using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Controls;

namespace SimServer.Rules
{
    [ComVisible(false)]
    internal class NotEmptyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return string.IsNullOrWhiteSpace((value ?? "").ToString())
                ? new ValidationResult(false, "Required")
                : ValidationResult.ValidResult;
        }
    }
}
