using System;
using System.Collections.Generic;

namespace UsbDeviceLibrary.Model
{
    /// <summary>
    /// Represents information about a USB drive, including details about its volumes.
    /// </summary>
    public class UsbDriveInfo : ICloneable
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

        public object Clone()
        {
            // Create a deep copy of the UsbDriveInfo
            var cloned = new UsbDriveInfo
            {
                DeviceName = this.DeviceName,
                DriveLetter = this.DriveLetter,
                SerialNumber = this.SerialNumber,
                Size = this.Size,
                Manufacturer = this.Manufacturer,
                Model = this.Model,
                InterfaceType = this.InterfaceType,
                Volumes = new List<VolumeInfo>(this.Volumes) // Assuming VolumeInfo is a class with value-like behavior or also implements ICloneable
            };
            return cloned;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            // Return details about the volumes instead of the drive
            if (Volumes.Count > 0)
            {
                var volumeDetails = new List<string>();
                foreach (var volume in Volumes)
                {
                    volumeDetails.Add($"{volume.Name} | {UsbDriveUtilities.FormatBytes(volume.Size)} | {UsbDriveUtilities.FormatBytes(volume.FreeSpace)} free | {volume.FileSystem}");
                }
                return string.Join(", ", volumeDetails);
            }
            return "No volumes detected on this drive.";
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