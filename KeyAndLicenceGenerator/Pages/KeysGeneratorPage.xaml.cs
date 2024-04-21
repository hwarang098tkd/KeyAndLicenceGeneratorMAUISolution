using System.Diagnostics;

namespace KeyAndLicenceGenerator.Pages;

public partial class KeysGeneratorPage : ContentPage
{
    public KeysGeneratorPage()
    {
        InitializeComponent();
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        Debug.WriteLine("Button Pressed");
    }
}