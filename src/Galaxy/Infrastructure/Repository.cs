using System;
using System.Collections.Generic;
using System.IO;
using Codestellation.Galaxy.Domain;
using Nejdb;
using NLog;

namespace Codestellation.Galaxy.Infrastructure
{
    public class Repository : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private Library _library;
        private Database _database;

        private Dictionary<Type, Collection> _collections;

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
        }

        private void CreateCollection(string collectionName, Type entityType)
        {
            var collection = _database.CreateCollection(collectionName, new CollectionOptions(false, false, 32, 32));
            _collections.Add(entityType, collection);
        }

        public Collection GetCollection<T>()
        {
            return _collections[typeof (T)];
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
            const string databaseName = "galaxy.db";

            var domainFolder = AppDomain.CurrentDomain.BaseDirectory;
            var databaseFolder = Path.Combine(domainFolder, "database");
            var dbPath = Path.Combine(databaseFolder, databaseName);

            Logger.Debug("Path to database: {0}", dbPath);
            
            if (!Directory.Exists(databaseFolder))
            {
                Directory.CreateDirectory(databaseFolder);
            }

            return dbPath;
        }
    }
}