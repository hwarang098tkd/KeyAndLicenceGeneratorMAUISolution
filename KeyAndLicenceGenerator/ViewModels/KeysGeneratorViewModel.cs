using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KeyAndLicenceGenerator.Models;
using KeyAndLicenceGenerator.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace KeyAndLicenceGenerator.ViewModels
{
    public partial class KeysGeneratorViewModel : ObservableObject
    {
        [ObservableProperty]
        private string countKeyslb;

        [ObservableProperty]
        private ObservableCollection<KeyFileInfo> keyFiles;

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
        private DateTime selectedDate = DateTime.Today.AddYears(1);

        public DateTime MinDate => DateTime.Today.AddDays(1);
        public DateTime MaxDate => DateTime.Today.AddYears(50);

        public bool IsFormValid => ValidateForm();
        private readonly string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$"; // Email validation pattern
        private readonly string specialCharsPattern = @"[^a-zA-Z0-9\s]"; // Detects special characters

        public KeysGeneratorViewModel()
        {
            KeyFiles = new ObservableCollection<KeyFileInfo>();
            LoadCollectionView();
        }

        private async Task LoadCollectionView()
        {
            keyFiles.Clear();
            await LoadKeyFiles();
        }

        private async Task LoadKeyFiles()
        {
            string appBasePath = AppDomain.CurrentDomain.BaseDirectory;
            string keysFolderPath = Path.Combine(appBasePath, "Keys");
            string pfxPassword = "vte3UW5YgHMgpgqIXe6mkP3wcI5gcKoF"; // The password used to export the pfx
            int counter = 0;
            var temporaryList = new List<KeyFileInfo>();

            foreach (string file in Directory.EnumerateFiles(keysFolderPath, "*.*", SearchOption.AllDirectories))
            {
                try
                {
                    X509Certificate2 cert;
                    if (file.EndsWith(".pfx"))
                    {
                        cert = new X509Certificate2(file, pfxPassword); // Load the PFX with the password
                        counter++;
                    }
                    /*else if (file.EndsWith(".cer"))
                    {
                        cert = new X509Certificate2(file); // Load the CER without a password
                    }*/
                    else
                    {
                        continue; // Skip non-certificate files
                    }

                    DateTime creationTime = File.GetCreationTime(file);

                    temporaryList.Add(new KeyFileInfo
                    {
                        FileName = Path.GetFileName(file),
                        FilePath = file,
                        ExpirationDate = cert.NotAfter,
                        CommonName = cert.GetNameInfo(X509NameType.SimpleName, false),
                        Email = cert.GetNameInfo(X509NameType.EmailName, false),
                        KeyType = file.EndsWith(".pfx") ? "Private" : "Public",
                        CreationDate = creationTime
                    });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to load certificate from {file}: {ex.Message}");
                }
            }

            // Sort the list by CreationDate descending
            var sortedList = temporaryList.OrderByDescending(k => k.CreationDate).ToList();

            // Clear and reload the observable collection
            KeyFiles.Clear();
            foreach (var fileInfo in sortedList)
            {
                KeyFiles.Add(fileInfo);
            }
            CountKeyslb = $"Βρέθηκαν {counter} κλειδιά";
        }

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
            KeyGeneratorService.GenerateAndSaveCertificate(commonName.ToUpper(), email, country.ToUpper(), selectedDate);
            Debug.WriteLine("Form is valid, proceeding with action.");
            await LoadCollectionView();
        }

        [RelayCommand]
        private async Task DeleteKeysAsync(KeyFileInfo keyFile)
        {
            Debug.WriteLine($"Deleting key: Pressed {keyFile.CreationDate}");
            await LoadCollectionView();
        }
    }
}