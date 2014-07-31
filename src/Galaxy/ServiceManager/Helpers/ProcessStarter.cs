using System;


namespace Codestellation.Galaxy.ServiceManager.Helpers
{
    public class ProcessStarter
    {
        public static string ExecuteWithParams(string exePath, string exeParams, out string error)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = exePath;
            startInfo.Arguments = exeParams;
            startInfo.Verb = "runas";
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            process.StartInfo = startInfo;
            process.Start();
            var output = process.StandardOutput.ReadToEnd();
            error = process.StandardError.ReadToEnd();

            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                ThrowExecuteIOPException(exePath, exeParams, process.ExitCode, output);
            }
            return output;
        }

        private static void ThrowExecuteIOPException(string exePath, string exeParams, int resultCode, string output)
        {
            var message = string.Format("execution of {0} with params {1} returned {2} with output {3}", exePath, exeParams, resultCode, output);
            throw new InvalidOperationException(message);
        }
    }
}