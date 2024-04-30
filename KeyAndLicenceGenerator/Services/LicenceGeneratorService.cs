using CommunityToolkit.Maui.Storage;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using UsbDeviceLibrary.Model;

namespace KeyAndLicenceGenerator.Services
{
    public class LicenceGeneratorService
    {
        private readonly IFileSaver fileSaver;

        public LicenceGeneratorService(IFileSaver fileSaver)
        {
            this.fileSaver = fileSaver;
        }

        public async Task<bool> GenerateAndSaveLicenseAsync(string companyName, string email, DateTime expiryDate, UsbDriveInfo usbDriveInfo, string pfxPath)
        {
            return await LicenceGeneratorAction(companyName, email, expiryDate, usbDriveInfo, pfxPath);
        }

        private async Task<bool> LicenceGeneratorAction(string companyName, string email, DateTime selectedExpiryDate, UsbDriveInfo usbDriveInfo, string pfxFilePath)
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
                bool resultSaveToFile = await SaveLicenseKeyToFile(companyName, licenseKey);
                Debug.WriteLine("License key generated and displayed successfully.");
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

        public async Task<bool> SaveLicenseKeyToFile(string companyName, string licenseKey)
        {
            string fileName = $"{companyName}.key";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(licenseKey));

            // Use FileSaver to save the file
            var cancellationToken = new CancellationToken();  // You might want to pass this as a parameter
            var fileSaverResult = await fileSaver.SaveAsync(fileName, stream, cancellationToken);

            if (fileSaverResult.IsSuccessful)
            {
                Debug.WriteLine($"License key saved to file: {fileSaverResult.FilePath}");
                return true;
            }
            else
            {
                if (fileSaverResult.Exception != null)
                    Debug.WriteLine($"Failed to save license key: {fileSaverResult.Exception.Message}");
                else
                    Debug.WriteLine("Save operation canceled by user.");

                return false;
            }
        }

        public string GetusbDeviceSerinaNumber(UsbDriveInfo usbDriveInfo)
        {
            return usbDriveInfo.SerialNumber;
        }
    }
}