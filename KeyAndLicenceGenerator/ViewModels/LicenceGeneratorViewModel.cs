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
        private DateTime selectedDate = DateTime.Today.AddDays(365);

        public static DateTime MinDate => DateTime.Today.AddDays(1);
        public static DateTime MaxDate => DateTime.Today.AddYears(50);

        [ObservableProperty]
        private int usbDeviceSelectedIndex = 0;

        [ObservableProperty]
        private List<string> usbDeviceNames;

        [ObservableProperty]
        private bool usbDeviceIsEnabled;

        public LicenceGeneratorViewModel()
        {
            UsbDeviceNames = new List<string>();
            LoadUsbDevicesAsync();
        }

#if WINDOWS

        [RelayCommand]
        public async Task LoadUsbDevicesAsync()
        {
            await FetchAndLoadUsbDevices();
        }

        private async Task FetchAndLoadUsbDevices()
        {
            try
            {
                UsbDeviceNames.Clear();
                List<UsbDriveInfo> usbDrives = UsbDriveSearcher.GetUsbDrives();
                if (usbDrives.Count != 0)
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
                UsbDeviceNames.Add("Error loading USB devices");
                Debug.WriteLine(ex.Message);
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