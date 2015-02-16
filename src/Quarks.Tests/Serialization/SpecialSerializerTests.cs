using System;
using System.Collections.Generic;
using Codestellation.Quarks.Serialization;
using NUnit.Framework;

namespace Codestellation.Quarks.Tests.Serialization
{
    [TestFixture]
    public class SpecialSerializerTests
    {
        [Test]
        public void Should_serialize_and_deserialize_serializable_types()
        {
            var random = new Random(2143513456);
            
            var base64String = SpecialSerializer.ToBase64(random);
            var deserializedRandom = SpecialSerializer.FromBase64<Random>(base64String);
            
            var expected = PrintRandoms(random);
            var actual = PrintRandoms(deserializedRandom);
            CollectionAssert.AreEqual(expected, actual);
        }

        private static List<int> PrintRandoms(Random random)
        {
            var randoms = new List<int>();
            for (int i = 0; i < 10; i++)
            {
                randoms.Add(random.Next());
            }

            Console.WriteLine("Original: {0}", string.Join(",", randoms));
            return randoms;
        }
    }
}