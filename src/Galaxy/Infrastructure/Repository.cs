using System;
using System.Collections.Generic;
using System.IO;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Domain.Notifications;
using Codestellation.Galaxy.Host;
using Codestellation.Quarks.IO;
using Nejdb;
using Newtonsoft.Json.Linq;
using NLog;

namespace Codestellation.Galaxy.Infrastructure
{
    public class Repository : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly CollectionOptions CollectionOptions = new CollectionOptions(false, false, 32, 32);

        private Library _library;
        private Database _database;

        private Dictionary<Type, Collection> _collections;

        private readonly DirectoryInfo _dataFolder;

        public Repository(HostConfig config)
        {
            _dataFolder = config.Data;
        }

        public Collection Feeds => _collections[typeof(NugetFeed)];
        public Collection Deployments => _collections[typeof(Deployment)];
        public Collection Options => _collections[typeof(Options)];
        public Collection Notifications => _collections[typeof(Notification)];

        public void Start()
        {
            _library = Library.Create();
            _database = _library.CreateDatabase();

            _collections = new Dictionary<Type, Collection>();

            var dbPath = GetDatabasePath();

            _database.Open(dbPath, Database.DefaultOpenMode | OpenMode.SyncTransactionToStorage);
            CreateCollection("users", typeof(User));
            CreateCollection("feeds", typeof(NugetFeed));
            CreateCollection("deployments", typeof(Deployment));
            CreateCollection("options", typeof(Options));
            CreateCollection("notification", typeof(Notification));
            MigrateDeployments();
        }

        private void MigrateDeployments()
        {
            var deployments = _collections[typeof(Deployment)];

            using (Query<JObject> query = deployments.CreateQuery<JObject>())
            using (Cursor<JObject> cursor = query.Execute())
            {
                foreach (JObject jDeployment in cursor)
                {
                    if (jDeployment.TryGetValue("ServiceFolders", out JToken jFolders))
                    {
                        dynamic serviceFolders = jFolders;
                        var deployment = jDeployment.ToObject<Deployment>(_database.Serializer);

                        deployment.Folders.BackupFolder = (FullPath)(string)serviceFolders.BackupFolder.FullPath;
                        deployment.Folders.Configs = (FullPath)(string)serviceFolders.Configs.FullPath;
                        deployment.Folders.Data = (FullPath)(string)serviceFolders.Data.FullPath;
                        deployment.Folders.DeployFolder = (FullPath)(string)serviceFolders.DeployFolder.FullPath;
                        deployment.Folders.DeployLogsFolder = (FullPath)(string)serviceFolders.DeployLogsFolder.FullPath;
                        deployment.Folders.Logs = (FullPath)(string)serviceFolders.Logs.FullPath;

                        using (var tx = deployments.BeginTransaction())
                        {
                            deployments.Save(deployment, false);
                            tx.Commit();
                        }
                    }
                }
            }
        }

        private void CreateCollection(string collectionName, Type entityType, bool clear = false)
        {
            var collection = _database.CreateCollection(collectionName, CollectionOptions);
            if (clear)
            {
                collection.Drop();
                collection = _database.CreateCollection(collectionName, CollectionOptions);
                Logger.Warn("Collection '{0}' was dropped", collectionName);
            }
            _collections.Add(entityType, collection);
        }

        public Collection GetCollection<T>()
        {
            if (_collections == null)
            {
                throw new InvalidOperationException("Please call start method before using repository");
            }
            return _collections[typeof(T)];
        }

        public void Dispose()
        {
            foreach (var collection in _collections.Values)
            {
                collection.Dispose();
            }

            _database.Close();
            _database.Dispose();
            _library.Dispose();
        }

        private string GetDatabasePath()
        {
            var folderPath = Folder.Combine(_dataFolder.FullName, "database");
            Folder.EnsureExists(folderPath);
            return Folder.Combine(folderPath, "galaxy.db");
        }
    }
}