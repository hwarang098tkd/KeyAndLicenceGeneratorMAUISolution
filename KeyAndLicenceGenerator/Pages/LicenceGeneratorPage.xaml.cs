using KeyAndLicenceGenerator.Services;
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
    }
}