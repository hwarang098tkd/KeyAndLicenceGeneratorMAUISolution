using KeyAndLicenceGenerator.ViewModels;

namespace KeyAndLicenceGenerator.Pages;

public partial class LicenceGeneratorPage : ContentPage
{
    public LicenceGeneratorPage()
    {
        InitializeComponent();
        this.BindingContext = new LicenceGeneratorViewModel();
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        var vm = BindingContext as LicenceGeneratorViewModel;
        vm?.FilterKeyFiles(e.NewTextValue);
    }
}