namespace Codestellation.Galaxy.ServiceManager.Helpers
{
    public class ExecutionResult
    {
        public int ExitCode { get; }
        public string StdOut { get; }
        public string StdError { get; }

        public ExecutionResult(int exitCode, string stdOut, string stdError)
        {
            ExitCode = exitCode;
            StdOut = stdOut;
            StdError = stdError;
        }
    }
}