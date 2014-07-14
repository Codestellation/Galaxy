using System;
using System.IO;

namespace Codestellation.Galaxy.ServiceManager.Helpers
{
    public class ProcessStarter
    {
        public static void ExecuteWithParams(string exePath, string exeParams)
        {
            string output;
            ExecuteWithParams(exePath, exeParams, out output);
        }

        public static void ExecuteWithParams(string exePath, string exeParams, out string output)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = exePath;
            startInfo.Arguments = exeParams;
            startInfo.Verb = "runas";
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            process.StartInfo = startInfo;
            process.Start();
            StreamReader streamReader = process.StandardOutput;
            output = streamReader.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
                ThrowExecuteIOPException(exePath, exeParams, process.ExitCode, output);
        }

        private static void ThrowExecuteIOPException(string exePath, string exeParams, int resultCode, string output)
        {
            throw new InvalidOperationException(
                string.Format("execution of {0} with params {1} returned {2} with output {3}", exePath, exeParams, resultCode, output));
        }
    }
}