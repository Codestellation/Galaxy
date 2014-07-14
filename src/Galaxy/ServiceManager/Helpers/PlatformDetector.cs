using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Codestellation.Galaxy.ServiceManager.Helpers
{
    public enum PlatformType
    {
        x64,
        x86,
        AnyCPU
    }

    public class PlatformDetector
    {
        private const string CorflagsPath = @"Microsoft SDKs\Windows\v7.0A\Bin\CorFlags.exe";

        public static PlatformType GetPlatform(string assemblyPath)
        {
            string output;
            var exePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), CorflagsPath);
            var exeParams = assemblyPath;

            ProcessStarter.ExecuteWithParams(exePath, exeParams, out output);

            Match matchFlagX86 = Regex.Match(output, @"32BIT[\s:]*(\d)+", RegexOptions.IgnoreCase);
            Match matchFlagPE = Regex.Match(output, @"PE[\s:]*([\w+]{4,})+", RegexOptions.IgnoreCase);

            if (matchFlagX86.Success && matchFlagPE.Success)
            {
                string flagX86 = matchFlagX86.Groups[1].Value;
                string flagPE = matchFlagPE.Groups[1].Value;

                if (flagX86.Equals("1") && flagPE.Equals("PE32"))
                {
                    return PlatformType.x86;
                }
                else if (flagX86.Equals("0") && flagPE.Equals("PE32+"))
                {
                    return PlatformType.x64;
                }
                else if (flagX86.Equals("0") && flagPE.Equals("PE32"))
                {
                    return PlatformType.AnyCPU;
                }
            }

            throw new InvalidOperationException(string.Format("Can't parse results of corflags on {0}, output: {1}", assemblyPath, output));
        }

        public static void ApplyPlatformToHost(PlatformType platform, string libPath)
        {
            // assuming we are running on x64
            // todo automate platform configuration according to current platform

            if (platform != PlatformType.x86)
            {
                return;
            }
            var exePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), CorflagsPath);
            var exeParams = string.Format("{0} {1}", libPath, "/32bit+");

            ProcessStarter.ExecuteWithParams(exePath, exeParams);
        }
    }
}