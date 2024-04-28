namespace KeyAndLicenceGenerator.Models
{
    public class CerFileInfo
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string CommonName { get; set; }
        public string Email { get; set; }
        public DateTime CreationDate { get; set; }
    }

    public class PfxFileInfo
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string CommonName { get; set; }
        public string Email { get; set; }
        public DateTime CreationDate { get; set; }
        public string KeyType { get; set; }
    }

    public class LicenseFileInfo
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string CustomerName { get; set; }
        public DateTime CreationDate { get; set; }
    }

    public class CertificatePairFileInfo
    {
        public CerFileInfo CerFile { get; set; }
        public PfxFileInfo PfxFile { get; set; }
    }
}