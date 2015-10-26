﻿using System;
using System.Collections.Generic;
using System.IO;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Domain.Notifications;
using Codestellation.Quarks.IO;
using Nejdb;
using NLog;

namespace Codestellation.Galaxy.Infrastructure
{
    public class Repository : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly CollectionOptions Options = new CollectionOptions(false, false, 32, 32);

        private Library _library;
        private Database _database;

        private Dictionary<Type, Collection> _collections;

        public static bool ClearUsersOnStart;

        public void Start()
        {
            _library = Library.Create();
            _database = _library.CreateDatabase();

            _collections = new Dictionary<Type, Collection>();

            var dbPath = GetDatabasePath();

            _database.Open(dbPath, Database.DefaultOpenMode | OpenMode.SyncTransactionToStorage);
            CreateCollection("users", typeof(User), ClearUsersOnStart);
            CreateCollection("feeds", typeof(NugetFeed));
            CreateCollection("deployments", typeof(Deployment));
            CreateCollection("options", typeof(Options));
            CreateCollection("notification", typeof(Notification));
        }

        private void CreateCollection(string collectionName, Type entityType, bool clear = false)
        {
            var collection = _database.CreateCollection(collectionName, Options);
            if (clear)
            {
                collection.Drop();
                collection = _database.CreateCollection(collectionName, Options);
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
            const string databaseName = "galaxy.db";

            var databaseFolder = Folder.Combine("data", "database");
            var dbPath = Path.Combine(databaseFolder, databaseName);

            Logger.Debug("Path to database: {0}", dbPath);
            Folder.EnsureExists(databaseFolder);
            return dbPath;
        }
    }
}