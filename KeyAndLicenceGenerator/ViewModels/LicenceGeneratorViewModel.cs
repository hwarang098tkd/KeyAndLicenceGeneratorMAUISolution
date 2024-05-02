using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KeyAndLicenceGenerator.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using UsbDeviceLibrary.Model;
using KeyAndLicenceGenerator.Models;
using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;

using System.Text;

#if WINDOWS

using UsbDeviceLibrary;

#endif

namespace KeyAndLicenceGenerator.ViewModels
{
    public partial class LicenceGeneratorViewModel : ObservableObject
    {
        private List<UsbDriveInfo> usbDeviceList { get; set; }

        [ObservableProperty]
        private double progressBarProgress;

        [ObservableProperty]
        private bool progressBarProgressVisible = false;

        [ObservableProperty]
        private string countKeyslb = "Βρέθηκαν 0 άδειες";

        [ObservableProperty]
        private bool headerIsVisible;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsFormValid))]
        private string commonName;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsFormValid))]
        private string email;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsFormValid))]
        private string country;

        public bool IsFormValid => ValidateFormChecker();

        [ObservableProperty]
        private int usbDeviceSelectedIndex = 0;

        [ObservableProperty]
        private ObservableCollection<string> usbDeviceNames;

        [ObservableProperty]
        private bool usbDeviceIsEnabled;

        [ObservableProperty]
        private ObservableCollection<CertificatePairModel> licenceFiles;

        [ObservableProperty]
        private ObservableCollection<PfxFileInfo> keyFiles;

        private readonly ObservableCollection<PfxFileInfo> _allKeyFiles = [];
        public ICommand FilterKeyFilesCommand { get; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsFormValid))]
        private PfxFileInfo selectedKeyFile;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsFormValid))]
        private DateTime selectedDate = DateTime.Today.Date.AddYears(1);

        public static DateTime MinDate => DateTime.Today.AddDays(1);
        public static DateTime MaxDate => DateTime.Today.AddYears(50);
        public ICommand SaveCommand { get; private set; }

        public LicenceGeneratorViewModel()
        {
            SaveCommand = new Command<LicenseFileInfo>(async (item) => await SaveFile(item));

            UsbDeviceNames = [];
            _ = LoadUsbDevicesAsync();
            KeyFiles = new ObservableCollection<PfxFileInfo>();
            LicenceFiles = new ObservableCollection<CertificatePairModel>();
            LoadKeyPicker();
            FilterKeyFilesCommand = new Command<string>(FilterKeyFiles);
            RefreshCollectionView();
        }

        partial void OnSelectedKeyFileChanged(PfxFileInfo value)
        {
            Debug.WriteLine(value?.FileName);
        }

        private void LoadCollectionView()
        {
            LicenceFiles.Clear();
            if (CertificateManager.CertificateModel.Count > 0)
            {
                CountKeyslb = $"Βρέθηκαν {CertificateManager.CertificateModel.Count} άδειες";
                HeaderIsVisible = true;
            }
            else
            {
                CountKeyslb = "Βρέθηκαν 0 άδειες";
                HeaderIsVisible = false;
            }
            foreach (var licenceFile in CertificateManager.CertificateModel)
            {
                LicenceFiles.Add(licenceFile);
            }
        }

        private async Task RefreshCollectionView()
        {
            CertificateManager.LoadCertificateLicences().Wait();
            LoadCollectionView();
        }

        private async Task SaveFile(LicenseFileInfo item)
        {
            var cancellationToken = new CancellationToken();
            using var stream = new MemoryStream(Encoding.Default.GetBytes("Data related to " + item.FilePath));
            var fileSaverResult = await FileSaver.Default.SaveAsync(item.FileName, stream, cancellationToken);
            if (fileSaverResult.IsSuccessful)
            {
                await Toast.Make($"The file '{item.FileName}' was saved successfully to location: {fileSaverResult.FilePath}").Show();
            }
            else
            {
                await Toast.Make($"The file was not saved successfully with error: {fileSaverResult.Exception?.Message}").Show();
            }
        }

        public void FilterKeyFiles(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                KeyFiles = new ObservableCollection<PfxFileInfo>(_allKeyFiles);
            }
            else
            {
                KeyFiles = new ObservableCollection<PfxFileInfo>(
                    _allKeyFiles.Where(pfx => pfx.FileName.ToLower().StartsWith(searchText.ToLower())));
            }
        }

        [RelayCommand]
        private void LoadKeyPicker()
        {
            KeyFiles.Clear();
            _allKeyFiles.Clear();
            foreach (var pair in CertificateManager.CertificatePairs)
            {
                _allKeyFiles.Add(pair.PfxFile);
            }
            KeyFiles = new ObservableCollection<PfxFileInfo>(_allKeyFiles);
        }

        public bool ValidateFormChecker()
        {
            var validationService = new ValidationFormService();
            bool result = validationService.ValidateForm(Email, CommonName, Country, SelectedDate);
            result = result && SelectedKeyFile?.FilePath != null;
            return result;
        }

#if WINDOWS

        [RelayCommand]
        public async Task LicenceGenerateAsync()
        {
            try
            {
                string selectedDevice = UsbDeviceNames[UsbDeviceSelectedIndex];
                string driveLetter = selectedDevice.Split('|')[0].Trim();
                string pathToPfxFile = SelectedKeyFile.FilePath;

                bool licenceGenerated = await LicenceGeneratorService.GenerateAndSaveLicenseAsync(
                    CommonName,
                    Email,
                    SelectedDate,
                    GetUsbDevice(driveLetter),
                    pathToPfxFile);

                if (licenceGenerated)
                {
                    Debug.WriteLine("License generated and saved successfully.");
                    RefreshCollectionView();
                }
                else
                {
                    Debug.WriteLine("Failed to generate and save license.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex, "Error occurred while generating and saving license.");
            }
        }

        private UsbDriveInfo? GetUsbDevice(string driveLetter)
        {
            foreach (UsbDriveInfo device in usbDeviceList)
            {
                if (device.DriveLetter == driveLetter)
                {
                    return device;
                }
            }
            return null;
        }

        //FORMAT
        [RelayCommand]
        public async Task DeviceFormatAsync()
        {
            if (UsbDeviceSelectedIndex < 0 || UsbDeviceSelectedIndex >= UsbDeviceNames.Count)
            {
                Debug.WriteLine("No USB device selected or invalid selection.");
                return;
            }

            string selectedDevice = UsbDeviceNames[UsbDeviceSelectedIndex];
            string driveLetter = selectedDevice.Split('|')[0].Trim();

            bool forFormatAnswer = await App.Current.MainPage.DisplayAlert(
                "ΠΡΟΣΟΧΗ",
                $"Κάνοντας Format διαγραφούν όλα τα δεδομένα στην συσκευή {driveLetter}!!!",
                "FORMAT",
                "ΑΚΥΡΩΣΗ");

            if (forFormatAnswer)
            {
                var formatService = new DeviceFormatService();
                var cts = new CancellationTokenSource();

                Task formattingTask = formatService.FormatDriveAsync(driveLetter, "EMMETRON_LC");
                Task simulateProgressTask = SimulateProgressAsync(cts.Token);

                ProgressBarProgressVisible = true;

                await formattingTask;  // Wait for the formatting to complete
                await LoadUsbDevicesAsync();
                cts.Cancel();  // Cancel the simulation task
                try
                {
                    await simulateProgressTask;  // Ensure the simulation task completes gracefully
                }
                catch (TaskCanceledException)
                {
                    Debug.WriteLine("Progress simulation cancelled.");
                }

                ProgressBarProgressVisible = false;
                ProgressBarProgress = 0;  // Reset progress after completion
                Debug.WriteLine("Format completed for " + driveLetter);
            }
            else
            {
                Debug.WriteLine("Format cancelled for " + driveLetter);
            }
        }

        private async Task SimulateProgressAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(200);  // Simulate ongoing activity
                ProgressBarProgress = (ProgressBarProgress + 0.1) % 1.0;  // Update progress cyclically
            }
        }

        [RelayCommand]
        public async Task LoadUsbDevicesAsync()
        {
            try
            {
                UsbDeviceNames.Clear();
                var usbDrives = usbDeviceList = await UsbDriveSearcher.GetUsbDrivesAsync(); // Correctly await the async call
                if (usbDrives.Count > 0) // Check if there are any USB devices found
                {
                    foreach (var drive in usbDrives)
                    {
                        foreach (var volume in drive.Volumes)
                        {
                            if (string.IsNullOrEmpty(volume.Name))
                            {
                                volume.Name = "UnNamed";  // Set to "UnNamed" if the volume name is empty
                            }
                        }
                        UsbDeviceNames.Add($"{drive.DriveLetter} | {drive}");
                        Debug.WriteLine($"UsbDeviceNames Found: {drive.DriveLetter} | {drive}");
                    }
                    Debug.WriteLine($"UsbDeviceNames Found: {UsbDeviceNames.Count}");
                    UsbDeviceIsEnabled = true;
                    // Set the first item as selected
                    UsbDeviceSelectedIndex = 1;
                }
                else
                {
                    UsbDeviceNames.Add("No USB devices found");
                    Debug.WriteLine("No USB devices found");
                    UsbDeviceIsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                UsbDeviceNames.Add("Error loading USB devices");
                Debug.WriteLine(ex.Message); // Log the exception
                UsbDeviceIsEnabled = false;
            }
        }

#else
        public void LoadUsbDevices()
        {
            UsbDeviceNames.Clear();
            UsbDeviceNames.Add("USB device functionality not supported on this platform."); // Inform user about lack of support
        }
#endif
    }
}