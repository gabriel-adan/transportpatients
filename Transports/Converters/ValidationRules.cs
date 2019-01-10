using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace Transports.Converters
{
    public class StringEmptyValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (!string.IsNullOrEmpty(value + ""))
                return ValidationResult.ValidResult;
            return new ValidationResult(false, "");
        }
    }

    public class HourValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                if (Regex.IsMatch(value + "", "^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$"))
                {
                    return ValidationResult.ValidResult;
                }
                else
                {
                    return new ValidationResult(false, "");
                }
            }
            catch (RegexMatchTimeoutException)
            {
                return new ValidationResult(false, "");
            }
            catch (ArgumentException)
            {
                return new ValidationResult(false, "");
            }
        }
    }
}
