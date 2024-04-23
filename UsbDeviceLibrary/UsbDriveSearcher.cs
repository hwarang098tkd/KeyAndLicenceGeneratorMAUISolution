using System.Management;
using UsbDeviceLibrary.Model;

namespace UsbDeviceLibrary
{
    /// <summary>
    /// Provides methods to search and retrieve information about connected USB drives.
    /// </summary>
    public class UsbDriveSearcher
    {
        /// <summary>
        /// Asynchronously retrieves a list of all connected USB drives with detailed information.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation and contains the list of USB drives.</returns>
        public static async Task<List<UsbDriveInfo>> GetUsbDrivesAsync()
        {
            return await Task.Run(() => GetUsbDrives());
        }

        /// <summary>
        /// Retrieves a list of all connected USB drives with detailed information.
        /// </summary>
        /// <returns>A list of USB drive information.</returns>
        public static List<UsbDriveInfo> GetUsbDrives()
        {
            List<UsbDriveInfo> drives = new List<UsbDriveInfo>();
            try
            {
                var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive WHERE InterfaceType='USB'");

                foreach (ManagementObject device in searcher.Get())
                {
                    UsbDriveInfo baseDriveInfo = CreateUsbDriveInfo(device);
                    PopulateVolumeInfo(baseDriveInfo, device["DeviceID"].ToString(), drives);
                }
            }
            catch (ManagementException ex)
            {
                // Handle exceptions related to WMI queries
                throw new InvalidOperationException("Failed to query USB drive information.", ex);
            }
            catch (Exception ex)
            {
                // Handle other types of exceptions that could occur (e.g., data conversion errors)
                throw new Exception("An unexpected error occurred while retrieving USB drive information.", ex);
            }

            return drives;
        }

        private static UsbDriveInfo CreateUsbDriveInfo(ManagementObject device)
        {
            return new UsbDriveInfo
            {
                DeviceName = device["Caption"].ToString(),
                SerialNumber = device["SerialNumber"]?.ToString() ?? "Unknown",
                Size = Convert.ToInt64(device["Size"]),
                Manufacturer = device["Manufacturer"]?.ToString() ?? "Unknown",
                Model = device["Model"].ToString(),
                InterfaceType = device["InterfaceType"].ToString()
            };
        }

        private static void PopulateVolumeInfo(UsbDriveInfo baseDriveInfo, string deviceId, List<UsbDriveInfo> drives)
        {
            var partitionSearcher = new ManagementObjectSearcher(
                $"ASSOCIATORS OF {{Win32_DiskDrive.DeviceID='{deviceId}'}} WHERE AssocClass = Win32_DiskDriveToDiskPartition");

            foreach (ManagementObject partition in partitionSearcher.Get())
            {
                var logicalSearcher = new ManagementObjectSearcher(
                    $"ASSOCIATORS OF {{Win32_DiskPartition.DeviceID='{partition["DeviceID"]}'}} WHERE AssocClass = Win32_LogicalDiskToPartition");

                foreach (ManagementObject logical in logicalSearcher.Get())
                {
                    // Clone the base drive info for each volume to treat them as separate devices
                    var clonedDriveInfo = (UsbDriveInfo)baseDriveInfo.Clone();
                    clonedDriveInfo.DriveLetter = logical["DeviceID"].ToString();
                    var volumeInfo = new VolumeInfo
                    {
                        Name = logical["VolumeName"].ToString(),
                        Size = Convert.ToInt64(logical["Size"]),
                        FreeSpace = Convert.ToInt64(logical["FreeSpace"]),
                        FileSystem = logical["FileSystem"].ToString()
                    };
                    clonedDriveInfo.Volumes.Clear(); // Clear any existing volumes
                    clonedDriveInfo.Volumes.Add(volumeInfo);

                    // Add the volume info as a separate device
                    drives.Add(clonedDriveInfo);
                }
            }
        }
    }
}