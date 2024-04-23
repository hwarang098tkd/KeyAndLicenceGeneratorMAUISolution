using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace KeyAndLicenceGenerator;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            // Initialize the .NET MAUI Community Toolkit by adding the below line of code
            .UseMauiCommunityToolkit()
            // After initializing the .NET MAUI Community Toolkit, optionally add additional fonts
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Font Awesome 6 Brands-Regular-400.otf", "FABrandsRegular");
                fonts.AddFont("Font Awesome 6 Free-Regular-400.otf", "FAFreeRegular");
                fonts.AddFont("Font Awesome 6 Free-Solid-900.otf", "FASolid");
            });
#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}