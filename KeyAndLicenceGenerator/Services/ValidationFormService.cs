using System.Diagnostics;
using System.Text.RegularExpressions;

namespace KeyAndLicenceGenerator.Services
{
    public partial class ValidationFormService
    {
        private readonly string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$"; // Email validation pattern
        private readonly string specialCharsPattern = @"[^a-zA-Z0-9\s]"; // Detects special characters

        public bool ValidateForm(string email, string commonName, string country, DateTime selectedDate)
        {
            Debug.WriteLine($"Validating... Email: {email}, Common Name: {commonName}, Country: {country}, Date: {selectedDate}");

            // Validate email
            bool isValidEmail = !string.IsNullOrWhiteSpace(email) &&
                                Regex.IsMatch(email, emailPattern) &&
                                email.Contains('@') && email.Contains('.');

            // Validate common name and country for minimum length and no special characters
            bool isCommonNameValid = !string.IsNullOrWhiteSpace(commonName) &&
            commonName.Length >= 3 &&
                                     !Regex.IsMatch(commonName, specialCharsPattern);

            bool isCountryValid = !string.IsNullOrWhiteSpace(country) &&
                                  country.Length >= 3 &&
                                  !Regex.IsMatch(country, specialCharsPattern);

            // Validate date
            bool isDateValid = selectedDate > DateTime.Today;

            // Overall validity
            bool isValid = isValidEmail && isCommonNameValid && isCountryValid && isDateValid;
            Debug.WriteLine($"Is form valid: {isValid}");
            return isValid;
        }
    }
}