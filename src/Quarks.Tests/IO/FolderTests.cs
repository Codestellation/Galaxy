using System.IO;
using System.Threading;
using Codestellation.Quarks.IO;
using NUnit.Framework;

namespace Codestellation.Quarks.Tests.IO
{
    [TestFixture]
    public class FolderTests
    {
        private const string FolderName = "Foo";
        private readonly string _fullPath = Path.Combine(Folder.BasePath, FolderName);

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_fullPath))
            {
                Directory.Delete(_fullPath);
            }
            WaitUntilDeleted();
        }

        [Test]
        public void Should_create_directory_if_it_not_exists()
        {
            //when
            Folder.EnsureExists(FolderName);
            //then
            bool exists = Directory.Exists(_fullPath);
            Assert.That(exists, Is.True);
        }

        [Test]
        public void Should_not_throw_if_directory_exists()
        {
            //when
            Directory.CreateDirectory(_fullPath);
            //then
            Assert.DoesNotThrow(() => Folder.EnsureExists(FolderName));
        }

        [Test]
        public void Should_delete_directory_if_exists()
        {
            //given
            Directory.CreateDirectory(_fullPath);

            //when
            Folder.EnsureDeleted(_fullPath);

            WaitUntilDeleted();

            //then
            bool exists = Directory.Exists(_fullPath);
            Assert.That(exists, Is.False);
        }

        [Test]
        public void Should_not_throw_if_directory_not_exists()
        {
            //then
            Assert.DoesNotThrow(() => Folder.EnsureDeleted(_fullPath));
        }

        [Test]
        public void Should_fully_qualify_not_rooted_path()
        {
            //when
            string path = Folder.Combine("Foo");
            
            //then
            Assert.That(path, Is.StringStarting(Folder.BasePath));
        }

        [Test]
        public void Should_not_fully_qualify_rooted_path()
        {
            //when
            string path = Folder.Combine(@"C:\Foo", "Bar");
            //then
            Assert.That(path, Is.EqualTo(@"C:\Foo\Bar"));
        }

        [Test]
        public void Should_enumerate_folders()
        {
            //when
            var folders = Folder.EnumerateFolders(@"C:\");
            //then
            Assert.That(folders, Is.Not.Null.And.Not.Empty);
        }
        

        //It takes some time to delete directory after Directory.Delete called. We have to wait,IOException is thrown otherwise.
        private void WaitUntilDeleted()
        {
            while (Directory.Exists(_fullPath))
            {
                Thread.Sleep(10);
            }
        }
    }
}