using KeyAndLicenceGenerator.ViewModels;
using System.Diagnostics;

#if WINDOWS

using UsbDeviceLibrary;

#endif

namespace KeyAndLicenceGenerator.Pages;

public partial class LicenceGeneratorPage : ContentPage
{
    public LicenceGeneratorPage()
    {
        InitializeComponent();
        this.BindingContext = new LicenceGeneratorViewModel();
    }

    private void OnFormatClicked(object sender, EventArgs e)
    {
        Debug.WriteLine("Format Button Clicked");
    }

    private void LicenceClicked(object sender, EventArgs e)
    {
        Debug.WriteLine("Licence Button Clicked");
    }
}