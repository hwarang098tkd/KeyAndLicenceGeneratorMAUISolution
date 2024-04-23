using Newtonsoft.Json;
using UsbDeviceLib.Model;

namespace UsbDeviceLib
{
    /// <summary>
    /// Provides utilities for extracting and formatting information about USB drives connected to a Windows system.
    /// This class includes methods for retrieving detailed device information, formatting that information into readable strings,
    /// and serializing the information to JSON format. It serves as a helper class for applications that need to manage or display
    /// data about USB storage devices.
    /// </summary>
    public static class UsbDriveUtilities
    {
        /// <summary>
        /// Generates a detailed list of properties for a specified USB drive including its volumes.
        /// This method ensures all critical information is presented clearly, with safeguards against null values.
        /// The output is structured for easy reading, making it suitable for logging or displaying in user interfaces.
        /// </summary>
        /// <param name="drive">The USB drive for which information is to be retrieved.</param>
        /// <returns>A list of strings, each containing key details about the USB drive and its volumes, formatted for display.</returns>
        public static List<string> GetUsbDriveInfo(UsbDriveInfo drive)
        {
            // Initialize the list of strings with the basic USB drive information.
            var driveInfo = new List<string>
            {
                $"Device Name: {drive.DeviceName}",
                $"Drive Letter: {drive.DriveLetter}",
                $"Serial Number: {drive.SerialNumber ?? "Unknown"}",  // Safeguard against null serial numbers
                $"Total Size: {FormatBytes(drive.Size)}",
                $"Manufacturer: {drive.Manufacturer ?? "Unknown"}",  // Safeguard against null manufacturers
                $"Model: {drive.Model ?? "Unknown"}",  // Safeguard against null models
                $"Interface Type: {drive.InterfaceType ?? "Unknown"}",  // Safeguard against null interface types
                "Volumes:"
            };

            // Check if the drive contains any volumes and add their details to the list.
            if (drive.Volumes != null && drive.Volumes.Count > 0)
            {
                foreach (var volume in drive.Volumes)
                {
                    driveInfo.Add($"  Volume Name: {volume.Name ?? "Unnamed"}, Size: {FormatBytes(volume.Size)}, Free Space: {FormatBytes(volume.FreeSpace)}, File System: {volume.FileSystem ?? "Unknown"}");
                }
            }
            else
            {
                driveInfo.Add("  No volumes detected on this drive.");
            }

            driveInfo.Add("");  // Add a newline for better separation between USB drive entries

            return driveInfo;
        }

        /// <summary>
        /// Converts the size in bytes to a more readable format with appropriate units.
        /// </summary>
        /// <param name="bytes">The size in bytes to format.</param>
        /// <returns>Formatted string representing the size in appropriate units.</returns>
        private static string FormatBytes(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };  // Array to hold size orders
            double formattedSize = bytes;
            int order = 0;

            while (formattedSize >= 1024 && order < sizes.Length - 1)
            {
                order++;
                formattedSize /= 1024;
            }

            return string.Format("{0:0.##} {1}", formattedSize, sizes[order]);
        }

        /// <summary>
        /// Serializes the list of USB drives to a JSON formatted string.
        /// </summary>
        /// <param name="usbDrives">The list of USB drives to serialize.</param>
        /// <returns>A string in JSON format representing the list of USB drives.</returns>
        public static string GetUsbDrivesAsJson(List<UsbDriveInfo> usbDrives)
        {
            return JsonConvert.SerializeObject(usbDrives, Newtonsoft.Json.Formatting.Indented,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
        }
    }
}