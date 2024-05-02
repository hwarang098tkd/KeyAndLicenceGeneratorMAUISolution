using KeyAndLicenceGenerator.Models;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace KeyAndLicenceGenerator.Services
{
    public static class CertificateManager
    {
        public static List<CertificatePairFileInfo> CertificatePairs { get; private set; } = new List<CertificatePairFileInfo>();
        public static List<LicenseFileInfo> CertificateLicences { get; private set; } = new List<LicenseFileInfo>();
        public static List<CertificatePairModel> CertificateModel { get; private set; } = new List<CertificatePairModel>();
        public static string KeysFolderPath { get; private set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Keys");
        public static string LicencesFolderPath { get; private set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Licences");
        public static string PfxPassword { get; private set; } = "vte3UW5YgHMgpgqIXe6mkP3wcI5gcKoF";

        public static async Task LoadCertificateLicences()
        {
            string licenseKeyDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Licences");

            try
            {
                if (!Directory.Exists(licenseKeyDirectoryPath))
                {
                    Debug.WriteLine("License key directory does not exist.");
                    return;
                }

                // Clear the existing certificate models
                CertificateModel.Clear();

                foreach (var pair in CertificatePairs)
                {
                    if (pair.CerFile == null || pair.PfxFile == null)
                    {
                        continue; // Skip if either .cer or .pfx file is missing
                    }

                    // Create a CertificatePairModel object for the pair
                    CertificatePairModel pairModel = new CertificatePairModel
                    {
                        CertificatePair = pair,
                        LicenseKeys = new List<LicenseFileInfo>()
                    };

                    foreach (string keyFilePath in Directory.EnumerateFiles(licenseKeyDirectoryPath, "*.key", SearchOption.AllDirectories))
                    {
                        try
                        {
                            // Verify and extract the key file with the associated certificate
                            LicenseFileInfo licenseFileInfo = VerifyAndExtractKeyFile(keyFilePath, pair.CerFile.FilePath);
                            if (licenseFileInfo != null)
                            {
                                // Add the license to the pair's LicenseKeys list
                                pairModel.LicenseKeys.Add(licenseFileInfo);
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Failed to verify key file {keyFilePath} with certificate {pair.CerFile.FilePath}: {ex.Message}");
                        }
                    }

                    // Add the pair model to the CertificateModel list
                    CertificateModel.Add(pairModel);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred while loading certificate licenses: {ex.Message}");
            }
        }

        public static LicenseFileInfo VerifyAndExtractKeyFile(string keyFilePath, string cerFilePath)
        {
            try
            {
                // Load the public key
                X509Certificate2 cert = new X509Certificate2(cerFilePath);
                RSA publicKey = cert.GetRSAPublicKey();

                // Read and split the key file content
                string encodedKey = File.ReadAllText(keyFilePath);
                string[] parts = encodedKey.Split('.');
                if (parts.Length != 2)
                {
                    Debug.WriteLine("The format of the key file is incorrect.");
                    return null;
                }

                // Decode the data and the signature
                byte[] data = Convert.FromBase64String(parts[0]);
                byte[] signature = Convert.FromBase64String(parts[1]);

                // Verify the signature
                bool isSignatureValid = publicKey.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                if (!isSignatureValid)
                {
                    Debug.WriteLine("Signature verification failed.");
                    return null;
                }

                // Process the data if needed
                string decodedData = Encoding.UTF8.GetString(data);
                return ParseLicenseInfo(decodedData, keyFilePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }

        private static LicenseFileInfo ParseLicenseInfo(string decodedData, string keyFilePath)
        {
            var parts = decodedData.Split('|');
            if (parts.Length < 4)
            {
                Debug.WriteLine("Decoded license data format is incorrect or incomplete.");
                return null;
            }

            try
            {
                return new LicenseFileInfo
                {
                    CustomerName = ExtractValueFromPart(parts[0]),
                    CustomerEmail = ExtractValueFromPart(parts[1]),
                    ExpirationDate = DateTime.Parse(ExtractValueFromPart(parts[2])),
                    UsbSerialNumberBound = ExtractValueFromPart(parts[3]),
                    FileName = Path.GetFileName(keyFilePath),
                    CreationDate = File.GetCreationTime(keyFilePath),
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error parsing license data: {ex.Message}");
                return null;
            }
        }

        private static string ExtractValueFromPart(string part)
        {
            int index = part.IndexOf(':');
            return index != -1 ? part.Substring(index + 1).Trim() : string.Empty;
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