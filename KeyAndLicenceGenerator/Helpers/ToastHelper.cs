using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.UI.Xaml;

namespace KeyAndLicenceGenerator.Helpers
{
    public static class ToastHelper
    {
        public static void ShowToast(string title, string content)
        {
            if (OperatingSystem.IsWindows())
            {
                new ToastContentBuilder()
                    .AddArgument("action", "viewConversation")
                    .AddText(title)
                    .AddText(content)
                    .Show();
            }
        }
    }
}