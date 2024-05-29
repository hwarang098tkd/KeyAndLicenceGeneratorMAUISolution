using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Storage;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace KeyAndLicenceGenerator
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            // Configure logging
            ConfigureLogging();
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
                });
            builder.Services.AddSingleton<IFileSaver>(FileSaver.Default);

#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }

        private static void ConfigureLogging()
        {
            var logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                // Logger for all levels
                .WriteTo.Debug(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm} [{Level}] {Message}{NewLine}{Exception}")
                // Logger for Debug, Warning, and Error levels
                .WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(evt => evt.Level == LogEventLevel.Debug || evt.Level == LogEventLevel.Warning || evt.Level == LogEventLevel.Error)
                    .WriteTo.File(
                        Path.Combine(logDirectory, "logDebug-.txt"),
                        rollingInterval: RollingInterval.Day,
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}")
                )
                // Logger for Info, Warnings
                .WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(evt => evt.Level == LogEventLevel.Information || evt.Level == LogEventLevel.Warning)
                    .WriteTo.File(
                        Path.Combine(logDirectory, "log-.txt"),
                        rollingInterval: RollingInterval.Day,
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm} [{Level}] {Message}{NewLine}{Exception}")
                )
                .CreateLogger();
        }
    }
}