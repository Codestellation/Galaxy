using Codestellation.Quarks.IO;
using Nejdb.Bson;

namespace Codestellation.Galaxy.Domain
{
    public class Options
    {
        public ObjectId Id { get; private set; }

        public Options()
        {
            FolderOptions = new FolderOptions();
        }

        public string RootDeployFolder { get; set; }

        public int PurgeLogsOlderThan { get; set; }

        public string ConsulAddress { get; set; }

        public string GetDeployFolder()
        {
            if (string.IsNullOrWhiteSpace(RootDeployFolder))
            {
                return Folder.BasePath;
            }

            return RootDeployFolder;
        }

        public FolderOptions FolderOptions { get; set; }
    }
}