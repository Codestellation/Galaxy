﻿using System;
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

        public OverrideFiles(string serviceFolder, string deploymentFilesFolder,  FileList skipList)
        {
            _serviceFolder = serviceFolder;
            _deploymentFilesFolder = deploymentFilesFolder;
            _skipList = skipList;
        }

        public void Execute(TextWriter buildLog)
        {
            _buildLog = buildLog;

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
            var files = Folder.EnumerateFiles(_deploymentFilesFolder);

            foreach (var file in files)
            {
                var destination = Path.Combine(_serviceFolder, file.Name);
                CopyFileToDestination(file.FullName, destination);
            }
        }

        private void CopyFileToDestination(string source, string destination)
        {
            try
            {
                _buildLog.Write("Copy '{0}' to '{1}'. ", source, destination);

                if (_skipList.IsMatched(destination))
                {
                    _buildLog.WriteLine("Skipped.");
                    return;
                }

                if (File.Exists(destination))
                {
                    File.Delete(destination);
                }

                File.Copy(source, destination, true);
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
