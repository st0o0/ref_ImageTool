using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace ImageTool.ValidationRules
{
    public class SearchValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string input = value as string;
            if (!string.IsNullOrEmpty(input))
            {
                if (Regex.IsMatch(input, "^[0-9]*$"))
                {
                    if (int.TryParse(input, out _))
                    {
                        return ValidationResult.ValidResult;
                    }
                    else
                    {
                        return new ValidationResult(false, "Nur Zahlen");
                    }
                }
                else
                {
                    if (Regex.IsMatch(input, "^[a-zA-Z]*[^_\\s]+$"))
                    {
                        return ValidationResult.ValidResult;
                    }
                    else
                    {
                        return new ValidationResult(false, "Ungültiges Zeichen");
                    }
                }
            }
            return ValidationResult.ValidResult;
        }
    }
}