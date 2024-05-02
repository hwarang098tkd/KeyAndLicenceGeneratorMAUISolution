using KeyAndLicenceGenerator.Models;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace KeyAndLicenceGenerator.Services
{
    public static class CertificateManager
    {
        public static List<CertificatePairFileInfo> CertificatePairs { get; private set; } = new List<CertificatePairFileInfo>();

        public static List<LicenseFileInfo> CertificateLicences { get; private set; } = new List<LicenseFileInfo>();
        public static string KeysFolderPath { get; private set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Keys");
        public static string LicencesFolderPath { get; private set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Licences");
        public static string PfxPassword { get; private set; } = "vte3UW5YgHMgpgqIXe6mkP3wcI5gcKoF";

        public static async Task<List<LicenseFileInfo>> LoadLicenseKeys()
        {
            string licenseKeyDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Licences");
            List<LicenseFileInfo> loadedLicenses = new List<LicenseFileInfo>();

            try
            {
                if (!Directory.Exists(licenseKeyDirectoryPath))
                {
                    Debug.WriteLine("License key directory does not exist.");
                    return loadedLicenses; // Return an empty list if the directory doesn't exist
                }

                foreach (string file in Directory.EnumerateFiles(licenseKeyDirectoryPath, "*.key", SearchOption.AllDirectories))
                {
                    try
                    {
                        string encodedLicenseKey = await File.ReadAllTextAsync(file);
                        // Decode or process the license key if necessary
                        var LicenseFileInfo = DecodeLicenseKey(encodedLicenseKey);
                        if (LicenseFileInfo != null)
                        {
                            loadedLicenses.Add(LicenseFileInfo);
                        }
                    }
                    catch (Exception readEx)
                    {
                        Debug.WriteLine($"Failed to read or process license key file {file}: {readEx.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred while loading license keys: {ex.Message}");
            }

            return loadedLicenses;
        }

        private static LicenseFileInfo DecodeLicenseKey(string encodedKey)
        {
            try
            {
                // Decode the base64 string to get the raw license info text
                byte[] keyBytes = Convert.FromBase64String(encodedKey);
                string keyText = Encoding.UTF8.GetString(keyBytes);

                // Split the keyText into components
                var parts = keyText.Split('|');
                if (parts.Length < 4) // Make sure all parts are present
                {
                    Debug.WriteLine("Decoded license key format is incorrect or incomplete.");
                    return null;
                }

                // Extract individual parts based on the known structure
                string companyName = ExtractValueFromPart(parts[0]);
                string email = ExtractValueFromPart(parts[1]);
                DateTime expDate = DateTime.Parse(ExtractValueFromPart(parts[2]));
                string serialNumber = ExtractValueFromPart(parts[3]);

                // Create the LicenseFileInfo object
                var info = new LicenseFileInfo
                {
                    CustomerName = companyName,
                    CustomerEmail = email,
                    ExpirationDate = expDate,
                    SerialNumber = serialNumber,
                    LicenseText = keyText  // Optionally store the entire decoded text
                };

                return info;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error decoding license key: {ex.Message}");
                return null;
            }
        }

        private static string ExtractValueFromPart(string part)
        {
            var keyValue = part.Split(':');
            if (keyValue.Length == 2)
            {
                return keyValue[1].Trim();
            }
            return string.Empty;  // Return empty if format is not as expected
        }

        public static async Task LoadCertificatePairs()
        {
            try
            {
                // Ensure directory exists
                if (!Directory.Exists(KeysFolderPath))
                {
                    Directory.CreateDirectory(KeysFolderPath);  // Optionally create the directory if it does not exist
                    // Or handle the case where the directory does not exist (e.g., log and return)
                    Debug.WriteLine("Keys directory does not exist and was created.");
                    return;  // Optionally return if no directory existed initially
                }

                CertificatePairs.Clear();

                // Iterate through each subdirectory in the Keys folder
                foreach (var subDir in Directory.GetDirectories(KeysFolderPath))
                {
                    CerFileInfo cerFile = null;
                    PfxFileInfo pfxFile = null;

                    // Process each file within the subdirectory
                    foreach (string file in Directory.EnumerateFiles(subDir, "*.*", SearchOption.TopDirectoryOnly))
                    {
                        X509Certificate2 cert = null;
                        if (file.EndsWith(".pfx"))
                        {
                            cert = new X509Certificate2(file, PfxPassword);
                            pfxFile = new PfxFileInfo
                            {
                                FileName = Path.GetFileName(file),
                                FilePath = file,
                                ExpirationDate = cert.NotAfter,
                                CommonName = cert.GetNameInfo(X509NameType.SimpleName, false),
                                Email = cert.GetNameInfo(X509NameType.EmailName, false),
                                CreationDate = File.GetCreationTime(file),
                            };
                        }
                        else if (file.EndsWith(".cer"))
                        {
                            cert = new X509Certificate2(file);
                            cerFile = new CerFileInfo
                            {
                                FileName = Path.GetFileName(file),
                                FilePath = file,
                                ExpirationDate = cert.NotAfter,
                                CommonName = cert.GetNameInfo(X509NameType.SimpleName, false),
                                Email = cert.GetNameInfo(X509NameType.EmailName, false),
                                CreationDate = File.GetCreationTime(file)
                            };
                        }
                    }

                    // If both .cer and .pfx files are found in the same directory, pair them
                    if (cerFile != null && pfxFile != null)
                    {
                        CertificatePairs.Add(new CertificatePairFileInfo
                        {
                            CerFile = cerFile,
                            PfxFile = pfxFile
                        });
                    }
                }
                Debug.WriteLine("File Pairs listed");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred while loading certificate pairs: {ex.Message}");
                // Handle exceptions or rethrow to be handled higher up in the call stack
                throw;  // Consider rethrowing if you want the calling method to handle the exception
            }
        }
    }
}