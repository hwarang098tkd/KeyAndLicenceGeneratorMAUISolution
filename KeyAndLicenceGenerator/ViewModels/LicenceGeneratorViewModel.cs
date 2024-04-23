using CommunityToolkit.Mvvm.ComponentModel;
using UsbDeviceLib;

namespace KeyAndLicenceGenerator.ViewModels
{
    public class LicenceGeneratorViewModel : ObservableObject
    {
        private List<string> _usbDeviceNames;

        public List<string> UsbDeviceNames
        {
            get => _usbDeviceNames;
            set => SetProperty(ref _usbDeviceNames, value);
        }

        public LicenceGeneratorViewModel()
        {
            UsbDeviceNames = new List<string> { "Επιλέξτε Συσκευή" }; // Default item
            LoadUsbDevices(); // Load USB devices at initialization
        }

#if WINDOWS

        public async void LoadUsbDevices()
        {
            UsbDeviceNames.Clear();
            UsbDeviceNames.Add("Επιλέξτε Συσκευή"); // Always add the default select item first

            try
            {
                var usbDrives = await UsbDriveSearcher.GetUsbDrivesAsync();
                if (usbDrives.Any()) // Check if there are any USB devices found
                {
                    foreach (var drive in usbDrives)
                    {
                        UsbDeviceNames.Add(drive.ToString()); // Assuming ToString() returns a meaningful string
                    }
                }
                else
                {
                    UsbDeviceNames.Add("No USB devices found"); // Inform user if no devices are found
                }
            }
            catch (Exception ex)
            {
                // Log or handle errors during USB drive search
                UsbDeviceNames.Add("Error loading USB devices");
                Console.WriteLine(ex.Message); // Consider using a logging framework or MAUI's built-in logging
            }
        }

#else
public void LoadUsbDevices()
{
    UsbDeviceNames.Clear();
    UsbDeviceNames.Add("Επιλέξτε Συσκευή"); // Add the default select item
    UsbDeviceNames.Add("USB device functionality not supported on this platform."); // Inform user about lack of support
}
#endif
    }
}