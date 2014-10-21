using System;
using System.IO;
using Codestellation.Galaxy.Domain;
using Codestellation.Quarks.IO;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class OverrideFiles : IOperation
    {
        private readonly string _serviceFolder;
        private readonly string _deploymentFilesFolder;
        private readonly FileList _skipList;
        private TextWriter _buildLog;

        public OverrideFiles(string serviceFolder, string deploymentFilesFolder, FileList skipList)
        {
            _serviceFolder = serviceFolder;
            _deploymentFilesFolder = deploymentFilesFolder;
            _skipList = skipList;
        }

        public void Execute(DeploymentTaskContext context)
        {
            _buildLog = context.BuildLog;

            if (Folder.Exists(_deploymentFilesFolder))
            {
                CopyFiles();
            }
            else
            {
                _buildLog.WriteLine("No files to override.");
            }
        }

        private void CopyFiles()
        {
            var files = Folder.EnumerateFilesRecursive(_deploymentFilesFolder);

            _buildLog.WriteLine("Found {0} files to override.", files.Length);

            foreach (var file in files)
            {
                var destination = BuildDestination(file);

                CopyFileToDestination(file, destination);
            }
        }

        private FileInfo BuildDestination(FileInfo file)
        {
            var relativePath = file.FullName.Substring(_deploymentFilesFolder.Length + 1);

            var destinationPath = Folder.Combine(_serviceFolder, relativePath);

            var destination = new FileInfo(destinationPath);
            return destination;
        }

        private void CopyFileToDestination(FileInfo source, FileInfo destination)
        {
            try
            {
                _buildLog.Write("Copy '{0}' to '{1}'. ", source.FullName, destination.FullName);

                Folder.EnsureExists(destination.DirectoryName);

                if (_skipList.IsMatched(destination.Name))
                {
                    _buildLog.WriteLine("Skipped.");
                    return;
                }

                source.CopyTo(destination.FullName, overwrite: true);

                _buildLog.WriteLine("Ok");
            }
            catch (Exception ex)
            {
                _buildLog.WriteLine("Failed: {0}", ex);
                throw;
            }
        }
    }
}
