using System;
using System.IO;
using Nejdb;
using NLog;

namespace Codestellation.Galaxy.Infrastructure
{
    public class Collections : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private Library _library;
        private Database _database;

        public Collection Settings { get; private set; }
        public Collection Users { get; private set; }
        public Collection Feeds { get; private set; }
        public Collection ServiceApps { get; private set; }

        public void Start()
        {
            _library = Library.Create();
            _database = _library.CreateDatabase();

            var dbPath = GetDatabasePath();
            
            _database.Open(dbPath, Database.DefaultOpenMode | OpenMode.SyncTransactionToStorage);
            Settings = _database.CreateCollection("settings", new CollectionOptions(false, false, 32,32));
            Users = _database.CreateCollection("users", new CollectionOptions(false, false, 32, 32));
            Feeds = _database.CreateCollection("feeds", new CollectionOptions(false, false, 32, 32));
            ServiceApps = _database.CreateCollection("serviceapps", new CollectionOptions(false, false, 32, 32));
        }

        public void Dispose()
        {
            Dispose(Settings);
            Dispose(Users);
            Dispose(ServiceApps);
            
            _database.Close();
            Dispose(_database);
            Dispose(_library);
        }

        private void Dispose(IDisposable disposable)
        {
            if (disposable != null)
            {
                disposable.Dispose();
            }
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