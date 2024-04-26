using CommunityToolkit.Maui;
using KeyAndLicenceGenerator.Interfaces;
using Microsoft.Extensions.Logging;

namespace KeyAndLicenceGenerator
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Font Awesome 6 Brands-Regular-400.otf", "FABrandsRegular");
                    fonts.AddFont("Font Awesome 6 Free-Regular-400.otf", "FAFreeRegular");
                    fonts.AddFont("Font Awesome 6 Free-Solid-900.otf", "FASolid");
                })
                .Services.AddSingleton<IFileSavePicker, WindowsFileSavePicker>();

#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}