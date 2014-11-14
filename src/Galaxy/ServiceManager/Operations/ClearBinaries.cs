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
        private DirectoryInfo _baseFolderInfo;

        public ClearBinaries(string basePath, FileList keepOnUpdate)
        {
            _keepOnUpdate = keepOnUpdate;
            _skippedFolders = new List<DirectoryInfo>();
            _baseFolderInfo = new DirectoryInfo(basePath);
        }

        public void Execute(DeploymentTaskContext context)
        {
            var entries = Directory
                .EnumerateFileSystemEntries(_baseFolderInfo.FullName, "*.*", SearchOption.AllDirectories)
                .Except(new[] { _baseFolderInfo.FullName });


            foreach (var path in entries)
            {
                if (Directory.Exists(path))
                {
                    DeleteFolder(path);
                    continue;
                }

                if (File.Exists(path))
                {
                    DeleteFile(path);
                }
            }
        }

        private void DeleteFolder(string path)
        {
            var info = new DirectoryInfo(path);

            var shouldSkip = _keepOnUpdate.IsMatched(path);

            if (shouldSkip)
            {
                _skippedFolders.Add(info);
                return;
            }

            foreach (var skippedFolder in _skippedFolders)
            {
                if (info.Parent != null &&  info.Parent.FullName.Equals(skippedFolder.FullName, StringComparison.OrdinalIgnoreCase))
                {
                    _skippedFolders.Add(info);
                    return;
                }
            }

            Directory.Delete(path, true);
        }

        private void DeleteFile(string path)
        {
            var info = new FileInfo(path);

            var shouldSkip = _keepOnUpdate.IsMatched(path) || InSkippedFolder(info);

            if (shouldSkip)
            {
                return;
            }

            File.Delete(path);
        }

        private bool InSkippedFolder(FileInfo fileInfo)
        {
            var currentParent = fileInfo.Directory;
            while (currentParent != null && !_baseFolderInfo.FullName.Equals(currentParent.FullName))
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