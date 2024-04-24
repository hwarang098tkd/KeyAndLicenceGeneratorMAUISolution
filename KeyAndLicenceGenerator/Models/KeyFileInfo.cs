namespace KeyAndLicenceGenerator.Models
{
    public class KeyFileInfo
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string CommonName { get; set; }
        public string Email { get; set; }
        public DateTime CreationDate { get; set; }
        public string KeyType { get; set; }
    }
}