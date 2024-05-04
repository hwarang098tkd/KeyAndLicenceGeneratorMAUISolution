using CommunityToolkit.Maui.Storage;
using KeyAndLicenceGenerator.ViewModels;
using System.Diagnostics;
using System.Text;

namespace KeyAndLicenceGenerator.Pages;

public partial class LicenceGeneratorPage : ContentPage
{
    IFileSaver fileSaver;
    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
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
        try
        {
            using var stream = new MemoryStream(Encoding.Default.GetBytes("Have you subscribed to this amazing channel yet?!"));
            var path = await fileSaver.SaveAsync("subscribe.txt", stream, cancellationTokenSource.Token);

            Debug.WriteLine(path);
        }
        catch
        {
            // Exception thrown when user cancels
        }
    }

}