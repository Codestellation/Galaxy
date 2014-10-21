using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using Nejdb.Bson;

namespace Codestellation.Galaxy.WebEnd.Models
{
    public class DeploymentFilesModel
    {
        public ObjectId DeploymentId { get; set; }
        [Display(Name = "Put to", Prompt = "relative path")]
        public string PutTo { get; set; }

        public DirectoryInfo FilesRootFolder { get; set; }

        public FileInfo[] Files { get; set; }

        public DeploymentFilesModel(ObjectId deploymentId, DirectoryInfo filesRootFolder,  FileInfo[] files)
        {
            DeploymentId = deploymentId;
            FilesRootFolder = filesRootFolder;
            Files = files;
        }

        public IEnumerable<string> FilePaths
        {
            get
            {
                var cutSymbols = FilesRootFolder.FullName.Length + 1;
                return Files.Select(fileInfo => fileInfo.FullName.Substring(cutSymbols));
            }
        }
    }
}