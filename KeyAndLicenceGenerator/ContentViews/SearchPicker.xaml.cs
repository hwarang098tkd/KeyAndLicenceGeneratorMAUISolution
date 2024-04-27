using Microsoft.Maui.Handlers;

namespace KeyAndLicenceGenerator.ContentViews;

public partial class SearchPicker : ContentView
{
    public SearchPicker()
    {
        InitializeComponent();

#if WINDOWS
        // Applying handler modifications only to filterEntry
        filterEntry.HandlerChanged += OnFilterEntryHandlerChanged;
#endif
    }

#if WINDOWS
    private void OnFilterEntryHandlerChanged(object sender, EventArgs e)
    {
        // Check if the handler is set and it's the Windows platform
        if (filterEntry.Handler is EntryHandler handler)
        {
            if (handler.PlatformView is Microsoft.UI.Xaml.Controls.TextBox textBox)
            {
                textBox.BorderThickness = new Microsoft.UI.Xaml.Thickness(0); // No border
                // Optionally, adjust other properties here
            }
        }
    }
#endif
}
