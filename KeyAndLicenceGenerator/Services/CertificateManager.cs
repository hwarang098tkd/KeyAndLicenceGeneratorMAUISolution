﻿using KeyAndLicenceGenerator.Models;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

namespace KeyAndLicenceGenerator.Services
{
    public static class CertificateManager
    {
        public static List<CertificatePairFileInfo> CertificatePairs { get; private set; } = new List<CertificatePairFileInfo>();

        public static string KeysFolderPath { get; private set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Keys");
        public static string PfxPassword { get; private set; } = "vte3UW5YgHMgpgqIXe6mkP3wcI5gcKoF";

        public static async Task LoadCertificatePairs()
        {
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
                            KeyType = "Private"
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
    }
}