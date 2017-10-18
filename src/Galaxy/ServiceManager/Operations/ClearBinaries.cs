using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Codestellation.Galaxy.Domain;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class ClearBinaries : IOperation
    {
        private FileList _keepOnUpdate;
        private List<DirectoryInfo> _skippedFolders;
        private DirectoryInfo _deployFolderInfo;

        public void Execute(DeploymentTaskContext context)
        {
            _skippedFolders = new List<DirectoryInfo>();
            _keepOnUpdate = context.KeepOnUpdate;
            _deployFolderInfo = new DirectoryInfo((string)context.Folders.DeployFolder);

            var buildLog = context.BuildLog;
            if (!_deployFolderInfo.Exists)
            {
                buildLog.WriteLine("Directory '{0}' does not exists. Clear binaries skipped", _deployFolderInfo.FullName);
                return;
            }

            Delete(buildLog);
        }

        private void Delete(TextWriter buildLog)
        {
            var entries = Directory
                .EnumerateFileSystemEntries(_deployFolderInfo.FullName, "*.*", SearchOption.AllDirectories)
                .Except(new[] { _deployFolderInfo.FullName });

            foreach (var path in entries)
            {
                if (Directory.Exists(path))
                {
                    DeleteFolder(path, buildLog);
                    continue;
                }

                if (buildLog != null && File.Exists(path))
                {
                    DeleteFile(path, buildLog);
                }
            }
        }

        private void DeleteFolder(string path, TextWriter buildLog)
        {
            var info = new DirectoryInfo(path);

            var shouldSkip = _keepOnUpdate.IsMatched(path);

            if (shouldSkip)
            {
                _skippedFolders.Add(info);
                buildLog.WriteLine("Directory '{0}' is skipped on clear binaries due to keep on update list", info.FullName);
                return;
            }

            foreach (var skippedFolder in _skippedFolders)
            {
                if (info.Parent != null && info.Parent.FullName.Equals(skippedFolder.FullName, StringComparison.OrdinalIgnoreCase))
                {
                    _skippedFolders.Add(info);
                    return;
                }
            }

            Directory.Delete(path, true);
            buildLog.WriteLine("Directory '{0}' was deleted", info.FullName);
        }

        private void DeleteFile(string path, TextWriter buildLog)
        {
            var info = new FileInfo(path);

            var shouldSkip = _keepOnUpdate.IsMatched(path) || InSkippedFolder(info);

            if (shouldSkip)
            {
                buildLog.WriteLine("File '{0}' is skipped on clear binaries due to keep on update list", info.FullName);
                return;
            }

            File.Delete(path);
            buildLog.WriteLine("File '{0}' was deleted", info.FullName);
        }

        private bool InSkippedFolder(FileInfo fileInfo)
        {
            var currentParent = fileInfo.Directory;
            while (currentParent != null && !_deployFolderInfo.FullName.Equals(currentParent.FullName))
            {
                if (_skippedFolders.Any(x => x.FullName.Equals(currentParent.FullName)))
                {
                    return true;
                }
                currentParent = currentParent.Parent;
            }

            return false;
        }
    }
}