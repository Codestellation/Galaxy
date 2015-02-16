using System;
using Codestellation.Quarks.Collections;
using NUnit.Framework;

namespace Codestellation.Quarks.Tests.Collections
{
    [TestFixture]
    public class AlgorithmExtensionsTests
    {
        [Test]
        public void Should_shuffle_array()
        {
            var array = new[] {1, 2, 3, 4, 5};
            var sourceArray = new int[array.Length];
            Array.Copy(array, sourceArray, sourceArray.Length);
            var shuffledArray = array.Shuffle();

            Assert.That(shuffledArray, Is.EquivalentTo(sourceArray));
        }
    }
}