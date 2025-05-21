using System.Diagnostics;

namespace Gosuji.API
{
    public class SG
    {
        public static async Task<string> GetVersion()
        {
            // Process process = new();
            // process.StartInfo = new()
            // {
            //     FileName = "git",
            //     Arguments = "rev-parse HEAD",
            //     RedirectStandardOutput = true,
            //     RedirectStandardError = true,
            //     UseShellExecute = false,
            //     CreateNoWindow = true
            // };

            // process.Start();

            // string version = (await process.StandardOutput.ReadToEndAsync()).Trim()[..5]; // First 5 chars of commit hash
            // string? error = (await process.StandardError.ReadToEndAsync()).Trim();

            // await process.WaitForExitAsync();
            // process.Dispose();

            // if (!string.IsNullOrEmpty(error))
            // {
            //     version = "1.0";
            //     Console.WriteLine($"SG.GetVersion Error: {error}");
            // }

            // return version;

            return "0.4.1";
        }
    }
}
