// In your Interfaces folder
namespace KeyAndLicenceGenerator.Interfaces
{
    public interface IFileSavePicker
    {
        Task<string> SaveFileAsync(string filename, string content);
    }
}