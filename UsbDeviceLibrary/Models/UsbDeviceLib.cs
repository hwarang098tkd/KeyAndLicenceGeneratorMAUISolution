using System.Collections.Generic;

namespace UsbDeviceLib.Model
{
    /// <summary>
    /// Represents information about a USB drive, including details about its volumes.
    /// </summary>
    public class UsbDriveInfo
    {
        /// <summary>Gets or sets the name of the device.</summary>
        public string DeviceName { get; set; }

        /// <summary>Gets or sets the drive letter associated with the USB drive.</summary>
        public string DriveLetter { get; set; }

        /// <summary>Gets or sets the serial number of the USB drive.</summary>
        public string SerialNumber { get; set; }

        /// <summary>Gets or sets the total size of the USB drive in bytes.</summary>
        public long Size { get; set; }

        /// <summary>Gets or sets the manufacturer of the USB drive.</summary>
        public string Manufacturer { get; set; }

        /// <summary>Gets or sets the model of the USB drive.</summary>
        public string Model { get; set; }

        /// <summary>Gets or sets the interface type of the USB drive, e.g., USB.</summary>
        public string InterfaceType { get; set; }

        /// <summary>Gets or sets the list of volumes on the USB drive.</summary>
        public List<VolumeInfo> Volumes { get; set; }

        /// <summary>
        /// Initializes a new instance of the UsbDriveInfo class.
        /// </summary>
        public UsbDriveInfo()
        {
            Volumes = new List<VolumeInfo>();
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            var volumeDescriptions = string.Empty;
            foreach (var volume in Volumes)
            {
                volumeDescriptions += $" {volume.ToString()} |";
            }
            volumeDescriptions = volumeDescriptions.TrimEnd('|');
            return $"{DriveLetter} | {DeviceName} | {Model} | {InterfaceType} | {volumeDescriptions}";
        }
    }

    /// <summary>
    /// Represents information about a volume on a USB drive.
    /// </summary>
    public class VolumeInfo
    {
        /// <summary>Gets or sets the name of the volume.</summary>
        public string Name { get; set; }

        /// <summary>Gets or sets the total size of the volume in bytes.</summary>
        public long Size { get; set; }

        /// <summary>Gets or sets the free space available on the volume in bytes.</summary>
        public long FreeSpace { get; set; }

        /// <summary>Gets or sets the file system type of the volume, e.g., NTFS or FAT32.</summary>
        public string FileSystem { get; set; }

        /// <summary>
        /// Returns a string that represents the current volume.
        /// </summary>
        /// <returns>A string that represents the current volume.</returns>
        public override string ToString()
        {
            return $"{Name} | {Size} bytes | {FreeSpace} bytes free | {FileSystem}";
        }
    }
}