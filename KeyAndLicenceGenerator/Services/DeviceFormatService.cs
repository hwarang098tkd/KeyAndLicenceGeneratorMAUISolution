using Serilog;
using System.Diagnostics;

namespace KeyAndLicenceGenerator.Services
{
    internal class DeviceFormatService
    {
        public async Task FormatDriveAsync(string driveLetter, string volumeLabel)
        {
            try
            {
                // Example command to format drive with volume label (this is pseudo-code)
                string arguments = $"/c format {driveLetter} /FS:NTFS /V:{volumeLabel} /Q /X /Y";
                ProcessStartInfo startInfo = new ProcessStartInfo("cmd.exe", arguments)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true  // Added to capture standard error
                };

                using (var process = Process.Start(startInfo))
                {
                    if (process != null)
                    {
                        string output = await process.StandardOutput.ReadToEndAsync(); // Reading output
                        string errors = await process.StandardError.ReadToEndAsync();  // Reading errors
                        await process.WaitForExitAsync();

                        if (!string.IsNullOrEmpty(errors))
                        {
                            Log.Information($"Error formatting drive {driveLetter}: {errors}");
                        }
                        else
                        {
                            Log.Information($"Drive {driveLetter} formatted successfully with label {volumeLabel}.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to format drive {driveLetter}: {ex.Message}");
                throw;
            }
        }
    }
}