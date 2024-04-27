using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KeyAndLicenceGenerator.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using UsbDeviceLibrary.Model;

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

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsFormValid))]
        private DateTime selectedDate = DateTime.Today.Date.AddYears(1);

        public static DateTime MinDate => DateTime.Today.AddDays(1);
        public static DateTime MaxDate => DateTime.Today.AddYears(50);

        public bool IsFormValid => ValidateFormChecker();

        [ObservableProperty]
        private int usbDeviceSelectedIndex = 0;

        [ObservableProperty]
        private ObservableCollection<string> usbDeviceNames;

        [ObservableProperty]
        private bool usbDeviceIsEnabled;

        public LicenceGeneratorViewModel()
        {
            UsbDeviceNames = [];
            _ = LoadUsbDevicesAsync();
        }

        public bool ValidateFormChecker()
        {
            var validationService = new ValidationFormService();
            var result = validationService.ValidateForm(Email, CommonName, Country, SelectedDate);
            return result;
        }

#if WINDOWS

        [RelayCommand]
        public async Task LicenceGenerateAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(CommonName) || string.IsNullOrWhiteSpace(Email))
                {
                    Debug.WriteLine("Company name and email are required.");
                    return;
                }

                if (UsbDeviceSelectedIndex < 0 || UsbDeviceSelectedIndex >= UsbDeviceNames.Count)
                {
                    Debug.WriteLine("No USB device selected or invalid selection.");
                    return;
                }

                string selectedDevice = UsbDeviceNames[UsbDeviceSelectedIndex];
                string driveLetter = selectedDevice.Split('|')[0].Trim();

                var licenceGeneratorService = new LicenceGeneratorService();

                bool licenceGenerated = await licenceGeneratorService.GenerateAndSaveLicense(
                    CommonName,
                    Email,
                    SelectedDate,
                    GetUsbDevice(driveLetter),
                    "path_to_pfx_file");

                if (licenceGenerated)
                {
                    Debug.WriteLine("License generated and saved successfully.");
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
            // Return null if no matching device is found after checking the whole list
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