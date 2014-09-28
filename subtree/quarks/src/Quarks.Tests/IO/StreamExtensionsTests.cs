using System;
using System.IO;
using Codestellation.Quarks.IO;
using Codestellation.Quarks.Resources;
using NUnit.Framework;

namespace Codestellation.Quarks.Tests.IO
{
    [TestFixture]
    public class StreamExtensionsTests
    {
        private string _fileName;

        [SetUp]
        public void SetUp()
        {
            _fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp.txt");
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(_fileName))
            {
                File.Delete(_fileName);
            }
        }

        [Test]
        public void Should_export_to_specified_file()
        {
            EmbeddedResource
                .EndsWith("embeddedsample.txt", StringComparison.OrdinalIgnoreCase)
                .ExportTo(_fileName);

            Assert.That(File.Exists(_fileName), Is.True);
        }

        [Test]
        public void Should_throw_if_file_exists()
        {
            var stream = EmbeddedResource.EndsWith("embeddedsample.txt", StringComparison.OrdinalIgnoreCase);

            stream.ExportTo(_fileName);

            stream.Position = 0;

            Assert.Throws<IOException>(() =>  stream.ExportTo(_fileName));
        }

        [Test]
        public void Should_not_throw_if_overwrite_file_is_on()
        {
            var stream = EmbeddedResource.EndsWith("embeddedsample.txt", StringComparison.OrdinalIgnoreCase);

            stream.ExportTo(_fileName);

            stream.Position = 0;

            Assert.DoesNotThrow(() => stream.ExportTo(_fileName, overwrite: true));
        }

        [Test]
        public void Should_export_to_temp_file_if_overwrite_file_is_on()
        {
            var stream = EmbeddedResource.EndsWith("embeddedsample.txt", StringComparison.OrdinalIgnoreCase);

            _fileName = stream.ExportToTempFile();

            Assert.That(File.Exists(_fileName), Is.True);
        }
    }
}