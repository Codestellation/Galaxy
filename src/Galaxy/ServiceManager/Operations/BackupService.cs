using System.IO;
using Codestellation.Quarks.DateAndTime;
using Codestellation.Quarks.IO;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class BackupService : IOperation
    {
        private readonly string _serviceName;
        private readonly string _serviceFolder;
        private readonly string _backupFolder;
        private TextWriter _buildLog;

        public BackupService(string serviceName, string serviceFolder, string backupFolder)
        {
            _serviceName = serviceName;
            _serviceFolder = serviceFolder;
            _backupFolder = backupFolder;
        }

        public void Execute(TextWriter buildLog)
        {
            _buildLog = buildLog;

            // TODO: create base operation class and move all the necessary folder creation there
            Folder.EnsureExists(_backupFolder);

            if (IsNothingToBackup())
            {
                _buildLog.WriteLine("Empty service folder for {0}. Nothing to backup", _serviceName);
                return;
            }

            Backup();
        }

        private bool IsNothingToBackup()
        {
            return !Folder.Exists(_serviceFolder);
        }

        private void Backup()
        {
            var timestamp = Clock.UtcNow.ToString("yyyy-MM-dd HH-mm-ss");
            var currentBackupFolder = Path.Combine(_backupFolder, timestamp);

            Folder.Copy(_serviceFolder, currentBackupFolder);

            _buildLog.WriteLine("{0} backed up to {1}", _serviceName, currentBackupFolder);
        }
    }
}