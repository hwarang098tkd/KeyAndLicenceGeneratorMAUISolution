using System.Diagnostics;

#if WINDOWS

using UsbDeviceLib;

#endif

namespace KeyAndLicenceGenerator.Pages;

public partial class LicenceGeneratorPage : ContentPage
{
    public LicenceGeneratorPage()
    {
        InitializeComponent();
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