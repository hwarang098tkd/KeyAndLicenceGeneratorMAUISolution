using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;

#if WINDOWS

using UsbDeviceLibrary;
using UsbDeviceLibrary.Model;

#endif

namespace KeyAndLicenceGenerator.ViewModels
{
    public partial class LicenceGeneratorViewModel : ObservableObject
    {
        [ObservableProperty]
        private string commonName;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string country;

        [ObservableProperty]
        private DateTime selectedDate;

        public static DateTime MinDate => DateTime.Today.AddDays(1);
        public static DateTime MaxDate => DateTime.Today.AddYears(50);

        [ObservableProperty]
        private int usbDeviceSelectedIndex = 0;  // Initialize to select the first item

        [ObservableProperty]
        private List<string> usbDeviceNames;

        [ObservableProperty]
        private bool usbDeviceIsEnabled;

        public LicenceGeneratorViewModel()
        {
            UsbDeviceNames = new List<string>(); // Correct initialization
            LoadUsbDevicesAsync(); // Load USB devices at initialization
        }

#if WINDOWS

        [RelayCommand]
        public async Task LoadUsbDevicesAsync()
        {
            try
            {
                UsbDeviceNames.Clear();
                List<UsbDriveInfo> usbDrives = UsbDriveSearcher.GetUsbDrives();
                if (usbDrives.Count != 0) // Check if there are any USB devices found
                {
                    foreach (var drive in usbDrives)
                    {
                        UsbDeviceNames.Add($"{drive.DriveLetter} | {drive}");
                        Debug.WriteLine($"UsbDeviceNames Found: {drive.DriveLetter} | {drive}");
                    }
                    Debug.WriteLine($"UsbDeviceNames Found: {UsbDeviceNames.Count}");
                    UsbDeviceIsEnabled = true;
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
                // Log or handle errors during USB drive search
                UsbDeviceNames.Add("Error loading USB devices");
                Debug.WriteLine(ex.Message); // Consider using a logging framework or MAUI's built-in logging
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