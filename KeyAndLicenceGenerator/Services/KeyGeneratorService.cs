using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace KeyAndLicenceGenerator.Services
{
    internal class KeyGeneratorService
    {
        public static void GenerateAndSaveCertificate(string commonName, string email, string country, DateTime selectedDate)
        {
            try
            {
                var distinguishedName = new X500DistinguishedName($"CN={commonName}, E={email}, C={country}");

                using (var rsa = RSA.Create(2048)) // Adjust key size as needed
                {
                    var request = new CertificateRequest(distinguishedName, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                    var certificate = request.CreateSelfSigned(DateTimeOffset.Now, selectedDate);

                    string pfxPassword = "vte3UW5YgHMgpgqIXe6mkP3wcI5gcKoF";
                    var pfxBytes = certificate.Export(X509ContentType.Pfx, pfxPassword);

                    // Format the current date and time
                    string dateTimeFolder = DateTime.Now.ToString("yyyy_M_dd_HH'h'_mm'm'_ss's'");

                    // Determine the folder path using application's base directory
                    string appBasePath = AppDomain.CurrentDomain.BaseDirectory;
                    string folderPath = Path.Combine(appBasePath, "Keys", dateTimeFolder);

                    // Ensure the directory exists
                    Directory.CreateDirectory(folderPath);

                    // Define file names
                    string pfxFilename = $"{commonName}_Key.pfx";
                    string cerFilename = $"{commonName}_PublicKey.cer";

                    // Save the PFX file
                    File.WriteAllBytes(Path.Combine(folderPath, pfxFilename), pfxBytes);

                    // Save the CER file
                    var cerBytes = certificate.Export(X509ContentType.Cert);
                    File.WriteAllBytes(Path.Combine(folderPath, cerFilename), cerBytes);

                    Debug.WriteLine($"The certificate files have been saved in the following directory: {folderPath}");
                }
            }
            catch (IOException ex)
            {
                Debug.WriteLine($"An IO error occurred: {ex.Message}");
            }
            catch (CryptographicException ex)
            {
                Debug.WriteLine($"A cryptographic error occurred: {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}