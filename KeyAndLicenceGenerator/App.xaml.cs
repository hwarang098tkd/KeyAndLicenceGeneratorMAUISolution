using Microsoft.Toolkit.Uwp.Notifications;

namespace KeyAndLicenceGenerator;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        // Add this inside the constructor or initialization method of your App
        ToastNotificationManagerCompat.OnActivated += notificationArgs =>
        {
            // Handle activation here
            // You can parse notificationArgs to respond to user interactions
        };
        MainPage = new AppShell();
    }

    protected override Window CreateWindow(IActivationState activationState)
    {
        var window = base.CreateWindow(activationState);

        const int newWidth = 1000;
        const int newHeight = 800;

        const int minWidth = 600;
        const int minHeight = 780;

        window.MinimumHeight = minHeight;
        window.MinimumWidth = minWidth;
        window.Width = newWidth;
        window.Height = newHeight;

        return window;
    }
}