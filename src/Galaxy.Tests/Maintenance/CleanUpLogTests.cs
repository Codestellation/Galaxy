using System;
using System.IO;
using Codestellation.Galaxy.ServiceManager;
using Codestellation.Galaxy.ServiceManager.Operations;
using Codestellation.Quarks.IO;
using NUnit.Framework;

namespace Codestellation.Galaxy.Tests.Maintenance
{
    [TestFixture]
    public class CleanUpLogTests
    {
        private string _folder;
        private string _oldLog;
        private string _newLog;

        [SetUp]
        public void Setup()
        {
            _folder = Folder.ToFullPath("Test");
            Folder.EnsureExists(_folder);

            _oldLog = Folder.Combine(_folder, "old.log");
            _newLog = Folder.Combine(_folder, "new.log");

            File.WriteAllText(_oldLog, "The data");
            File.WriteAllText(_newLog, "The data");

            File.SetLastWriteTime(_oldLog, DateTime.Now.AddDays(-20));
        }

        [TearDown]
        public void TearDown()
        {
            Folder.EnsureDeleted(_folder);
        }

        [Test]
        public void Will_remove_old_files_from_folder()
        {
            int timeToLive = 10;

            PerformCleanup(timeToLive);

            Assert.That(File.Exists(_oldLog), Is.False, "Old log was not deleted");
            Assert.That(File.Exists(_newLog), Is.True, "New log was deleted");
        }

        [Test]
        public void Will_not_remove_old_files_if_time_to_live_is_zero()
        {
            int timeToLive = 0;

            PerformCleanup(timeToLive);

            Assert.That(File.Exists(_oldLog), Is.True, "Old log was deleted");
            Assert.That(File.Exists(_newLog), Is.True, "New log was deleted");
        }

        private void PerformCleanup(int timeToLive)
        {
            var cleanUp = new CleanUpLog();

            var stringWriter = new StringWriter();
            var context = new DeploymentTaskContext(stringWriter);
            context.Parameters.LogFolders = new[] { _folder };

            context.Parameters.PurgeLogOlderThen = timeToLive;

            cleanUp.Execute(context);

            Console.WriteLine(stringWriter.ToString());
        }
    }
}