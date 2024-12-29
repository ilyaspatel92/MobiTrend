using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Mobi.Web.Utilities.Validations
{
    public class CustomEmailAttribute : ValidationAttribute
    {
        private const string EmailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success; // Null cases are handled by [Required].
            }

            var email = value.ToString();
            if (Regex.IsMatch(email, EmailRegex))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(ErrorMessage ?? "Invalid email address format.");
        }
    }

}
