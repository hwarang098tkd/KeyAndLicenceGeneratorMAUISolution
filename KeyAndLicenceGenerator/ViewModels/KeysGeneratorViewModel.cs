using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KeyAndLicenceGenerator.Models;
using KeyAndLicenceGenerator.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace KeyAndLicenceGenerator.ViewModels
{
    public partial class KeysGeneratorViewModel : ObservableObject
    {
        [ObservableProperty]
        private string countKeyslb;

        [ObservableProperty]
        private bool headerIsVisible;

        [ObservableProperty]
        private ObservableCollection<PfxFileInfo> keyFiles;

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

        public bool IsFormValid => ValidateFormChecker();

        public KeysGeneratorViewModel()
        {
            KeyFiles = new ObservableCollection<PfxFileInfo>();
            LoadCollectionView();
        }

        private void LoadCollectionView()
        {
            KeyFiles.Clear();
            foreach (var pair in CertificateManager.CertificatePairs)
            {
                KeyFiles.Add(pair.PfxFile);
            }
        }

        private async Task RefreshCollectionView()
        {
            CertificateManager.LoadCertificatePairs().Wait();
            LoadCollectionView();
        }
        public bool ValidateFormChecker()
        {
            var validationService = new ValidationFormService();
            var result = validationService.ValidateForm(Email, CommonName, Country, SelectedDate);
            return result;
        }

        [RelayCommand]
        public async Task GenerateKeys()
        {
            KeyGeneratorService.GenerateAndSaveCertificate(commonName.ToUpper(), email, country.ToUpper(), selectedDate);
            Debug.WriteLine("Form is valid, proceeding with action.");
            RefreshCollectionView();
        }

        [RelayCommand]
        private async Task DeleteKeysAsync(PfxFileInfo keyFile)
        {
            Debug.WriteLine($"Deleting key: Pressed {keyFile.CreationDate}");
            // Prompt the user for confirmation before deletion
            bool forDeleteAnswer = await App.Current.MainPage.DisplayAlert(
                "ΠΡΟΣΟΧΗ",
                $"Θέλετε να διαγράψετε το κλειδί {keyFile.CommonName} ;",
                "NAI",
                "ΑΚΥΡΩΣΗ");

            // Proceed with deletion if the user confirms
            if (forDeleteAnswer)
            {
                await DeleteKeysActionAsync(keyFile);  // Call the deletion method
                Debug.WriteLine("Deletion completed for " + keyFile.FileName);
            }
            else
            {
                Debug.WriteLine("Deletion cancelled for " + keyFile.FileName);
            }

            RefreshCollectionView();
        }

        private async Task DeleteKeysActionAsync(PfxFileInfo keyFile)
        {
            string appBasePath = AppDomain.CurrentDomain.BaseDirectory;
            string keysDirectoryPath = Path.Combine(appBasePath, "Keys");
            string targetFolderName = keyFile.CreationDate.ToString("yyyy_M_d_HH'h'_mm'm'_ss's'");
            string targetFolderPath = Path.Combine(keysDirectoryPath, targetFolderName);
            if (Directory.Exists(targetFolderPath))
            {
                try
                {
                    Directory.Delete(targetFolderPath, true);
                    Debug.WriteLine($"Successfully deleted folder: {targetFolderPath}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error deleting folder {targetFolderPath}: {ex.Message}");
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert(
                "ΠΡΟΣΟΧΗ",
                $"Δεν βρέθηκε το κλειδί {keyFile.CommonName} !!!",
                "OK");
                Debug.WriteLine($"No folder found matching the date {targetFolderName}");
            }
        }
    }
}