using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
            bool forFormatAnswer = await App.Current.MainPage.DisplayAlert(
               "ΠΡΟΣΟΧΗ",
               $"Κάνοντας Format διαγραφούν όλα τα δεδομένα στην συσκευή!!!",
               "FORMAT",
               "ΑΚΥΡΩΣΗ");

            // Proceed with deletion if the user confirms
            if (forFormatAnswer)
            {
                //call format method form the DeviceFormatService
                //also send the usbdevice info
                //the user selected the usb device in the
                /*< Picker x: Name = "usbDevicePicker"
                        MinimumWidthRequest = "400"
                        HorizontalOptions = "Center"
                        HorizontalTextAlignment = "Center"
                        ToolTipProperties.Text = "Επιλέξτε το USB για να αποθηκευτεί η άδεια. Το κλειδί θα εμπεριέχει το Serial Number της συσκευής USB."
                        ItemsSource = "{Binding UsbDeviceNames}"
                        SelectedIndex = "{Binding UsbDeviceSelectedIndex}"
                        IsEnabled = "{Binding UsbDeviceIsEnabled}"
                        Title = "Επιλέξτε Συσκευή:" >
                </ Picker >*/
                Debug.WriteLine("Format completed for ");
            }
            else
            {
                Debug.WriteLine("Format cancelled for ");
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