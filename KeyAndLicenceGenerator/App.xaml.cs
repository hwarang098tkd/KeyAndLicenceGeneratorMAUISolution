using KeyAndLicenceGenerator.Pages;
using KeyAndLicenceGenerator.Services;
using System.Globalization;

namespace KeyAndLicenceGenerator;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new AppShell();
        // Set the default language to Greek
        CertificateManager.LoadCertificatePairs().Wait();
        CertificateManager.LoadCertificateModels().Wait();
    }

    protected override Window CreateWindow(IActivationState activationState)
    {
        var window = base.CreateWindow(activationState);
        SetupInitialWindowDimensions(window);
        return window;
    }

    private void SetupInitialWindowDimensions(Window window)
    {
        // Default settings for production
        const int newWidth = 1000;
        const int newHeight = 800;
        const int minWidth = 600;
        const int minHeight = 780;

        window.MinimumHeight = minHeight;
        window.MinimumWidth = minWidth;
        window.Width = newWidth;
        window.Height = newHeight;

#if DEBUG
        // Debug settings to test window behavior under different dimensions
        ResizeWindow(window, 1200, 700, 10, 10);
#endif
    }

    public void ResizeWindow(Window window, int width, int height, int x, int y)
    {
        window.X = x;
        window.Y = y;
        window.Width = width;
        window.Height = height;
    }

    
}