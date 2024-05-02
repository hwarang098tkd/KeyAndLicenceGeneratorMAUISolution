using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using UsbDeviceLibrary.Model;

namespace KeyAndLicenceGenerator.Services
{
    internal class LicenceGeneratorService
    {
        public static async Task<bool> GenerateAndSaveLicenseAsync(string companyName, string email, DateTime expiryDate, UsbDriveInfo usbDriveInfo, string pfxPath)
        {
            return await LicenceGeneratorAction(companyName, email, expiryDate, usbDriveInfo, pfxPath);
        }

        private static async Task<bool> LicenceGeneratorAction(string companyName, string email, DateTime selectedExpiryDate, UsbDriveInfo usbDriveInfo, string pfxFilePath)
        {
            try
            {
                Debug.WriteLine("Generate Process STARTED.");
                Debug.WriteLine($"Loading private key from: {pfxFilePath}");

                string pfxPassword = "vte3UW5YgHMgpgqIXe6mkP3wcI5gcKoF";
                X509Certificate2 certificate = new(pfxFilePath, pfxPassword, X509KeyStorageFlags.Exportable);
                RSA privateRSA = certificate.GetRSAPrivateKey();
                Debug.WriteLine("Private key loaded successfully.");

                string usbDeviceSerinaNumber = GetusbDeviceSerinaNumber(usbDriveInfo);
                if (string.IsNullOrWhiteSpace(usbDeviceSerinaNumber))
                {
                    Debug.WriteLine("No USB device identifier found.");
                    return false;
                }
                Debug.WriteLine($"Using USB Device Identifier: {usbDeviceSerinaNumber}");

                DateTime certificateExpiry = certificate.NotAfter;
                if (selectedExpiryDate > certificateExpiry)
                {
                    return false;
                }

                string licenseInfo = $"Company Name: {companyName}|Email: {email}|Exp. Date: {selectedExpiryDate:yyyy-MM-dd}|SerialNumber: {usbDeviceSerinaNumber}";
                Debug.WriteLine($"License information: {licenseInfo}");

                byte[] dataToSign = Encoding.UTF8.GetBytes(licenseInfo);
                byte[] signature = privateRSA.SignData(dataToSign, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                string base64Signature = Convert.ToBase64String(signature);
                string base64LicenseInfo = Convert.ToBase64String(dataToSign);

                string licenseKey = $"{base64LicenseInfo}.{base64Signature}";
                bool resultSaveToUsb = SaveLicenseKeyToUSB(usbDriveInfo.DriveLetter, licenseKey);

                // Determine the folder path using application's base directory
                string dateTimeFolder = DateTime.Now.ToString("yyyy_MM_dd_HH'h'_mm'm'_ss's'");
                string appBasePath = AppDomain.CurrentDomain.BaseDirectory;
                string folderPath = Path.Combine(appBasePath, "Licences", dateTimeFolder);

                // Ensure the directory exists
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                    Debug.WriteLine("Licences directory was created successfully.");
                }

                // Save the license key file
                string pfxFilename = $"{companyName}.key";
                string fullPath = Path.Combine(folderPath, pfxFilename);
                await File.WriteAllTextAsync(fullPath, licenseKey);
                Debug.WriteLine($"License key saved successfully to: {fullPath}");

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex, "Error occurred while generating license key.");
                return false;
            }
        }

        private static bool SaveLicenseKeyToUSB(string driveLetter, string licenseKey)
        {
            try
            {
                string usbPath = $"{driveLetter}\\";
                string directoryPath = Path.Combine(usbPath, "LicenseKeys");
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                string filePath = Path.Combine(directoryPath, "license.key");
                File.WriteAllText(filePath, licenseKey);
                File.SetAttributes(filePath, File.GetAttributes(filePath) | FileAttributes.Hidden);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        public static string GetusbDeviceSerinaNumber(UsbDriveInfo usbDriveInfo)
        {
            return usbDriveInfo.SerialNumber;
        }
    }
}