using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Codestellation.Galaxy.ServiceManager.Operations;
using Codestellation.Quarks.DateAndTime;
using Codestellation.Quarks.IO;

namespace Codestellation.Galaxy.Maintenance
{
    public class CleanUpLog : IOperation
    {
        public const string LogFolders = "Maintenance.LogFolders";
        public const string PurgeLogOlderThen = "Maintenance.PurgeLogOlderThen";

        public void Execute(DeploymentTaskContext context)
        {
            string[] folders = context.Parameters.LogFolders;
            int timeToLive = context.Parameters.PurgeLogOlderThen;

            if (timeToLive <= 0)
            {
                context.BuildLog.WriteLine("Purge Logs Older than is set to {0}. Skipped.", timeToLive);
                return;
            }

            var lastDateToLive = Clock.UtcNow.AddDays(-timeToLive).Date;

            foreach (var folder in folders)
            {
                RemoveOldFiles(folder, lastDateToLive, context.BuildLog);
            }
        }

        private void RemoveOldFiles(string folder, DateTime lastDateToLive, TextWriter buildLog)
        {
            var files = Folder.EnumerateFiles(folder);

            foreach (var file in files.Where(f => f.LastWriteTime < lastDateToLive))
            {
                buildLog.WriteLine("File {0} last write {1}. Removed", file.FullName, file.LastWriteTime.ToString(CultureInfo.InvariantCulture));
                file.Delete();
            }
        }
    }
}