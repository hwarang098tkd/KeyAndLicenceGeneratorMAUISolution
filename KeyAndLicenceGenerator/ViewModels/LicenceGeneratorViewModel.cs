using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KeyAndLicenceGenerator.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

#if WINDOWS

using UsbDeviceLibrary;

#endif

namespace KeyAndLicenceGenerator.ViewModels
{
    public partial class LicenceGeneratorViewModel : ObservableObject
    {
        [ObservableProperty]
        private double progressBarProgress;

        [ObservableProperty]
        private bool progressBarProgressVisible = false;

        [ObservableProperty]
        private string countKeyslb = "Βρέθηκαν 0 άδειες";

        [ObservableProperty]
        private bool headerIsVisible;

        [ObservableProperty]
        private string commonName;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string country;

        [ObservableProperty]
        private DateTime selectedDate = DateTime.Today.Date.AddYears(1);

        public static DateTime MinDate => DateTime.Today.AddDays(1);
        public static DateTime MaxDate => DateTime.Today.AddYears(50);

        [ObservableProperty]
        private int usbDeviceSelectedIndex = 0;  // Initialize to select the first item

        [ObservableProperty]
        private ObservableCollection<string> usbDeviceNames;

        [ObservableProperty]
        private bool usbDeviceIsEnabled;

        public LicenceGeneratorViewModel()
        {
            UsbDeviceNames = new ObservableCollection<string>(); // Correct initialization
            LoadUsbDevicesAsync(); // Load USB devices at initialization
        }

#if WINDOWS

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
                var usbDrives = await UsbDriveSearcher.GetUsbDrivesAsync(); // Correctly await the async call
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