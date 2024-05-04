using CommunityToolkit.Maui.Storage;
using KeyAndLicenceGenerator.Models;
using KeyAndLicenceGenerator.Services;
using KeyAndLicenceGenerator.ViewModels;
using System.Diagnostics;
using System.Text;

namespace KeyAndLicenceGenerator.Pages;

public partial class LicenceGeneratorPage : ContentPage
{
    private IFileSaver fileSaver;
    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    // Parameterless constructor for use by MAUI
    public LicenceGeneratorPage()
    {
        InitializeComponent();
        // Manually retrieve the IFileSaver from the MauiApp's service provider
        this.fileSaver = (IFileSaver)MauiProgram.CreateMauiApp().Services.GetService(typeof(IFileSaver));

        this.BindingContext = new LicenceGeneratorViewModel();
    }

    // Constructor with IFileSaver dependency for direct usage
    public LicenceGeneratorPage(IFileSaver fileSaver)
    {
        InitializeComponent();
        this.fileSaver = fileSaver;
        this.BindingContext = new LicenceGeneratorViewModel();
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        var vm = BindingContext as LicenceGeneratorViewModel;
        vm?.FilterKeyFiles(e.NewTextValue);
    }

    private async void OnsaveClickevent(object sender, EventArgs e)
    {
        Debug.WriteLine("Clicked");
        if (sender is Button button && button.CommandParameter is LicenseFileInfo selectedItem)
        {
            try
            {
                // Find the matching LicenseFileInfo in the nested CertificateModel list
                LicenseFileInfo matchedLicense = null;
                foreach (var certPair in CertificateManager.CertificateModel)
                {
                    matchedLicense = certPair.LicenseKeys.FirstOrDefault(l => l.CreationDate == selectedItem.CreationDate);
                    if (matchedLicense != null)
                    {
                        break;
                    }
                }

                if (matchedLicense != null)
                {
                    // Open and read the file specified by the matchedLicense's FilePath
                    byte[] fileContents;
                    using (var fileStream = File.OpenRead(matchedLicense.FilePath))
                    {
                        fileContents = new byte[fileStream.Length];
                        await fileStream.ReadAsync(fileContents, 0, fileContents.Length);
                    }

                    // Create a memory stream from the file contents
                    using (var stream = new MemoryStream(fileContents))
                    {
                        // Save the file to a new location using the file saver service
                        var path = await fileSaver.SaveAsync(Path.GetFileName(matchedLicense.FilePath), stream, cancellationTokenSource.Token);
                        Debug.WriteLine($"File saved to: {path}");
                    }
                }
                else
                {
                    Debug.WriteLine("No matching license file found for the selected item.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}