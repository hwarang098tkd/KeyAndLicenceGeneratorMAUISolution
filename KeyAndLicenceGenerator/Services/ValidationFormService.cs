using System.Diagnostics;
using System.Text.RegularExpressions;

namespace KeyAndLicenceGenerator.Services
{
    public partial class ValidationFormService
    {
        private readonly string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        private readonly string specialCharsPattern = @"[^a-zA-Z0-9\s]";

        public bool ValidateForm(string email, string commonName, string country, DateTime selectedDate)
        {
            bool isValidEmail = !string.IsNullOrWhiteSpace(email) &&
                                Regex.IsMatch(email, emailPattern) &&
                                email.Contains('@') && email.Contains('.');

            bool isCommonNameValid = !string.IsNullOrWhiteSpace(commonName) &&
            commonName.Length >= 3 &&
                                     !Regex.IsMatch(commonName, specialCharsPattern);

            bool isCountryValid = !string.IsNullOrWhiteSpace(country) &&
                                  country.Length >= 3 &&
                                  !Regex.IsMatch(country, specialCharsPattern);

            bool isDateValid = selectedDate > DateTime.Today;

            bool isValid = isValidEmail && isCommonNameValid && isCountryValid && isDateValid;
            Debug.WriteLine($"Is form valid: {isValid}");
            return isValid;
        }
    }
}