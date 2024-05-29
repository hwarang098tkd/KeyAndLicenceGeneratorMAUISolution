using KeyAndLicenceGenerator.ViewModels;

namespace KeyAndLicenceGenerator.Pages;

public partial class KeysGeneratorPage : ContentPage
{
    public KeysGeneratorPage()
    {
        InitializeComponent();
        this.BindingContext = new KeysGeneratorViewModel();
    }
}