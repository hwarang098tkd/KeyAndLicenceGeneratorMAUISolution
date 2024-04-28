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

        public bool ValidateFormChecker()
        {
            var validationService = new ValidationFormService();
            var result = validationService.ValidateForm(Email, CommonName, Country, SelectedDate);
            return result;
        }

        //private async Task LoadKeyFiles()
        //{
        //    string appBasePath = AppDomain.CurrentDomain.BaseDirectory;
        //    string keysFolderPath = Path.Combine(appBasePath, "Keys");
        //    string pfxPassword = "vte3UW5YgHMgpgqIXe6mkP3wcI5gcKoF"; // The password used to export the pfx
        //    int counter = 0;
        //    var temporaryList = new List<PfxFileInfo>();

        //    foreach (string file in Directory.EnumerateFiles(keysFolderPath, "*.*", SearchOption.AllDirectories))
        //    {
        //        try
        //        {
        //            X509Certificate2 cert;
        //            if (file.EndsWith(".pfx"))
        //            {
        //                cert = new X509Certificate2(file, pfxPassword); // Load the PFX with the password
        //                counter++;
        //            }
        //            /*else if (file.EndsWith(".cer"))
        //            {
        //                cert = new X509Certificate2(file); // Load the CER without a password
        //            }*/
        //            else
        //            {
        //                continue; // Skip non-certificate files
        //            }

        //            DateTime creationTime = File.GetCreationTime(file);

        //            temporaryList.Add(new PfxFileInfo
        //            {
        //                FileName = Path.GetFileName(file).Replace("_Key.pfx", ""),
        //                FilePath = file,
        //                ExpirationDate = cert.NotAfter,
        //                CommonName = cert.GetNameInfo(X509NameType.SimpleName, false),
        //                Email = cert.GetNameInfo(X509NameType.EmailName, false),
        //                KeyType = file.EndsWith(".pfx") ? "Private" : "Public",
        //                CreationDate = creationTime
        //            });
        //        }
        //        catch (Exception ex)
        //        {
        //            Debug.WriteLine($"Failed to load certificate from {file}: {ex.Message}");
        //        }
        //    }

        //    // Sort the list by CreationDate descending
        //    var sortedList = temporaryList.OrderByDescending(k => k.CreationDate).ToList();

        //    // Clear and reload the observable collection
        //    KeyFiles.Clear();
        //    foreach (var fileInfo in sortedList)
        //    {
        //        KeyFiles.Add(fileInfo);
        //    }
        //    if (counter <= 0)
        //    {
        //        HeaderIsVisible = false;
        //    }
        //    else
        //    {
        //        HeaderIsVisible = true;
        //    }
        //    CountKeyslb = $"Βρέθηκαν {counter} κλειδιά";
        //}

        [RelayCommand]
        public async Task GenerateKeys()
        {
            KeyGeneratorService.GenerateAndSaveCertificate(commonName.ToUpper(), email, country.ToUpper(), selectedDate);
            Debug.WriteLine("Form is valid, proceeding with action.");
            LoadCollectionView();
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

            LoadCollectionView();
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