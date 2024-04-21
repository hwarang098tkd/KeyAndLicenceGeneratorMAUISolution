using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace KeyAndLicenceGenerator.ViewModels
{
    public partial class KeysGeneratorViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsFormValid))]
        private string commonName;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsFormValid))]
        private string email;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsFormValid))]
        private string country;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsFormValid))]
        private DateTime selectedDate;

        public DateTime MinDate => DateTime.Today.AddDays(1);
        public DateTime MaxDate => DateTime.Today.AddYears(50);

        public bool IsFormValid => ValidateForm();
        private readonly string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$"; // Email validation pattern
        private readonly string specialCharsPattern = @"[^a-zA-Z0-9\s]"; // Detects special characters

        private bool ValidateForm()
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


        [RelayCommand]
        public async Task GenerateKeys()
        {
            if (IsFormValid)
            {
                Debug.WriteLine("Form is valid, proceeding with action.");
                // Proceed with generating keys
            }
            else
            {
                Debug.WriteLine("Form is invalid, action aborted.");
                // Alert the user or log the error
                // This requires injecting a service for showing alerts, or handling it in the view.
            }
        }
    }
}