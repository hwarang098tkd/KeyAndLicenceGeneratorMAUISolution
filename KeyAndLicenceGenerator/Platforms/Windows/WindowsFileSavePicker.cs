using Windows.Storage.Pickers;
using Windows.Storage;
using KeyAndLicenceGenerator.Interfaces;

namespace KeyAndLicenceGenerator;

public class WindowsFileSavePicker : IFileSavePicker
{
    public async Task<string> SaveFileAsync(string filename, string content)
    {
        var picker = new FileSavePicker();
        picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
        picker.FileTypeChoices.Add("Key File", new List<string> { ".key" });
        picker.SuggestedFileName = filename;

        StorageFile file = await picker.PickSaveFileAsync();
        if (file != null)
        {
            await FileIO.WriteTextAsync(file, content);
            return file.Path;
        }

        return null;
    }
}