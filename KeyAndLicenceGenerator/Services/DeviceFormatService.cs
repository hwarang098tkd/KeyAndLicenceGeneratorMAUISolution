using System.Diagnostics;

namespace KeyAndLicenceGenerator.Services
{
    internal class DeviceFormatService
    {
        public async Task FormatDriveAsync(string driveLetter, string volumeLabel)
        {
            try
            {
                await Task.Delay(1000);
                // Example command to format drive with volume label (this is pseudo-code)
                string arguments = $"/c format {driveLetter}: /FS:NTFS /V:{volumeLabel} /Q /X /Y";
                ProcessStartInfo startInfo = new ProcessStartInfo("cmd.exe", arguments)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                };

                using (var process = Process.Start(startInfo))
                {
                    if (process != null)
                    {
                        await process.WaitForExitAsync();
                        Debug.WriteLine($"Drive {driveLetter} formatted successfully with label {volumeLabel}.");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to format drive {driveLetter}: {ex.Message}");
                throw;
            }
        }
    }
}