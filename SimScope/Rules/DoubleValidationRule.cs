using System.Runtime.InteropServices;
using System.Windows.Controls;

namespace SimServer.Rules
{
    [ComVisible(false)]
    internal class DoubleValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            var canConvert = double.TryParse(value as string, out _);
            return new ValidationResult(canConvert, "Invalid Number");
        }
    }
}
