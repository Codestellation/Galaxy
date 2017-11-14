using System;
using System.Collections.Generic;
using System.IO;
using Codestellation.Galaxy.Domain;
using Codestellation.Quarks.IO;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class MoveFolder : IOperation
    {
        public void Execute(DeploymentTaskContext context)
        {
            var actual = context.Folders;
            var folderName = (string)context.Parameters.FolderName;
            var folderValue = (FullPath)(string)context.Parameters.FolderValue;
            var errors = ValidateFolders(actual, folderName, folderValue);
            if (errors.Count > 0)
            {
                var message = "Could not move folders. Reasons: "
                    + Environment.NewLine
                    + string.Join(Environment.NewLine, errors);
                throw new InvalidOperationException(message);
            }
            MoveFolders(actual, folderName, folderValue, context);
        }

        private void MoveFolders(ServiceFolders actual, string folderName, FullPath path, DeploymentTaskContext context)
        {
            var cloned = actual.Clone();

            var source = (string)actual[folderName];
            var destination = path;

            var haveSameRoot = Path.GetPathRoot(source) == Path.GetPathRoot((string) destination);

            if (haveSameRoot)
            {
                MoveSimple(context.BuildLog, source, destination);
            }
            else
            {
                CopyAndDelete(context.BuildLog, source, destination);
            }

            cloned[folderName] = destination;
            context.NewFolders = cloned;
        }

        private static void MoveSimple(TextWriter buildLog, string source, FullPath destination)
        {
            buildLog.Write($"Moving from '{source}' to '{destination}'...");
            Directory.Move(source, (string) destination);
            buildLog.Write($"Ok");
            buildLog.WriteLine();
        }

        private void CopyAndDelete(TextWriter buildLog, string source, FullPath destination)
        {
            buildLog.Write($"Copy from '{source}' to '{destination}'...");
            Folder.Copy(source, (string) destination);
            buildLog.Write($"Ok");
            buildLog.WriteLine();

            buildLog.Write($"Delete source folder '{source}'");
            Folder.EnsureDeleted(source);
            buildLog.Write($"Ok");
            buildLog.WriteLine();
        }

        private List<string> ValidateFolders(ServiceFolders actual, string folderName, FullPath path)
        {
            var errors = new List<string>();
            var actualDictionary = actual.ToDictionary();

            if (!Path.IsPathRooted((string)path))
            {
                var error = $"Path must be rooted, not relative, but was '{path}'.";
                errors.Add(error);
                return errors;
            }

            if (!actualDictionary.TryGetValue(folderName, out string current))
            {
                //folder should be known to the service
                var error = $"Folder '{folderName}'('{path}') is unknown.";
                errors.Add(error);
                return errors;
            }

            if (Folder.Exists((string)path))
            {
                //target folder should not exist
                var error =
                    $"Folder '{folderName}'('{path}') already exists. Remove it before starting the operation.";
                errors.Add(error);
                return errors;
            }

            // folder path should be valid
            var result = Folder.EnsureExists((string)path);

            if (!result.Exists)
            {
                var error = $"Cannot create folder '{folderName}'('{path}'). Possibly invalid path.";
                errors.Add(error);
                return errors;
            }

            Folder.EnsureDeleted((string)path);
            return errors;
        }
    }
}