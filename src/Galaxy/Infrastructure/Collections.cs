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
        
        private Collection _connection;

        public void Start()
        {
            _library = Library.Create();
            _database = _library.CreateDatabase();

            var dbPath = GetDatabasePath();
            
            _database.Open(dbPath, Database.DefaultOpenMode | OpenMode.SyncTransactionToStorage);
            Settings = _database.CreateCollection("settings", CollectionOptions.None);
            Users = _database.CreateCollection("users", CollectionOptions.None);
            _connection = _database.CreateCollection("connection", CollectionOptions.None);
        }

        public void Dispose()
        {
            Dispose(Settings);
            Dispose(Users);
            
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
            const string databaseFolder = "database";

            const string databaseName = "galaxy.db";
            var domainFolder = AppDomain.CurrentDomain.BaseDirectory;

            var dbPath = Path.Combine(domainFolder, databaseFolder, databaseName);

            Logger.Debug("Path to database: {0}", dbPath);
            
            if (!Directory.Exists(databaseFolder))
            {
                Directory.CreateDirectory(databaseFolder);
            }

            return dbPath;
        }
    }
}