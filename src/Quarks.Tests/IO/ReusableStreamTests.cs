using System;
using System.IO;
using System.Reflection;
using Codestellation.Quarks.IO;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Codestellation.Quarks.Tests.IO
{
    [TestFixture]
    public class ReusableStreamTests
    {
        private byte[] _ethalonBuffer;
        private MemoryStream _ethalonStream;
        private byte[] _testBuffer;
        private ReusableStream _testStream;

        [SetUp]
        public void Setup()
        {
            const int bufferLength = 64 * 1024;
            _ethalonBuffer = new Byte[bufferLength];
            _ethalonStream = new MemoryStream(_ethalonBuffer);

            _testBuffer = new byte[bufferLength];
            _testStream = new ReusableStream(_testBuffer);
        }

        [Test]
        public void Should_generate_results_same_as_memory_stream_on_write()
        {
            var filename = Assembly.GetExecutingAssembly().Location;

            using (var fileStream = File.OpenRead(filename))
            {
                fileStream.CopyTo(_ethalonStream);

                fileStream.Seek(0, SeekOrigin.Begin);

                fileStream.CopyTo(_testStream);
            }
            CollectionAssert.AreEqual(_ethalonBuffer, _testBuffer);
        }

        [Test]
        public void Should_work_same_way_as_memory_stream_on_read()
        {
            var serializer = new JsonSerializer();

            var origin = new TestClass
            {
                Name = "FFasdf fasd  FASDfas   fadsfa  dafsdf  asdf a fsd ",
                CreatedAt = DateTime.Now,
                IsObsolete = true,
                Count = 31241325351
            };
            using (var writer = new JsonTextWriter(new StreamWriter(_ethalonStream)))
            {
                serializer.Serialize(writer, origin);
            }
            
            _testStream.ChangeBuffer(_ethalonBuffer);
            
            using (var reader = new JsonTextReader(new StreamReader(_testStream)))
            {
                var reburnished = serializer.Deserialize<TestClass>(reader);

                Assert.That(reburnished.Name, Is.EqualTo(origin.Name));
                Assert.That(reburnished.CreatedAt, Is.EqualTo(origin.CreatedAt));
                Assert.That(reburnished.IsObsolete, Is.EqualTo(origin.IsObsolete));
                Assert.That(reburnished.Count, Is.EqualTo(origin.Count));
            }
        }
    }

    public class TestClass
    {
        public string Name;

        public DateTime CreatedAt;
        
        public bool IsObsolete;

        public long Count;
    }
}