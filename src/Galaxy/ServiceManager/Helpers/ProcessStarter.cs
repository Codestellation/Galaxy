using System;
using System.Diagnostics;
using System.IO;

namespace Codestellation.Galaxy.ServiceManager.Helpers
{
    public class ProcessStarter
    {
        public static ExecutionResult ExecuteWithParams(string exePath, string exeParams, bool throwOnError = true)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = exePath;
            startInfo.WorkingDirectory = Path.GetDirectoryName(exePath);
            startInfo.Arguments = exeParams;
            startInfo.Verb = "runas";
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            process.StartInfo = startInfo;
            process.Start();
            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();

            process.WaitForExit();

            if (process.ExitCode != 0 && throwOnError)
            {
                ThrowExecuteIOPException(exePath, exeParams, process.ExitCode, output);
            }

            return new ExecutionResult(process.ExitCode, output, error);
        }

        private static void ThrowExecuteIOPException(string exePath, string exeParams, int resultCode, string output)
        {
            var message = $"execution of {exePath} with params {exeParams} returned {resultCode} with output {output}";
            throw new InvalidOperationException(message);
        }
    }
}