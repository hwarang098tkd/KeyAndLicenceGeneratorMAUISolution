using KeyAndLicenceGenerator.ViewModels;
using System.Diagnostics;

namespace KeyAndLicenceGenerator.Pages;

public partial class LicenceGeneratorPage : ContentPage
{
    public LicenceGeneratorPage()
    {
        InitializeComponent();
        this.BindingContext = new LicenceGeneratorViewModel();
    }

    private async void OnFormatClicked(object sender, EventArgs e)
    {
        Debug.WriteLine("Format Button Clicked");
    }

    private async void LicenceClicked(object sender, EventArgs e)
    {
        Debug.WriteLine("Licence Button Clicked");

        // Only execute this on Windows
#if WINDOWS
        Debug.WriteLine("Licence Button Clicked1");
        ShowWindowsToast("Licence Notification", "This is a toast notification for the Licence operation.");
#endif
    }

#if WINDOWS
    private void ShowWindowsToast(string title, string content)
    {
        // Make sure to use the correct namespaces and setup for toast notifications as discussed earlier.
        new Microsoft.Toolkit.Uwp.Notifications.ToastContentBuilder()
            .AddText(title)
            .AddText(content)
            .Show();
    }
#endif

}