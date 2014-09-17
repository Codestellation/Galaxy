using System;
using Codestellation.Quarks.Collections;
using NUnit.Framework;

namespace Codestellation.Quarks.Tests.Collections
{
    [TestFixture]
    public class CollectionExtensionsTests
    {
        private Tuple<int>[] _unsorted;

        [SetUp]
        public void SetUp()
        {
            _unsorted = new[] {Tuple.Create(2), Tuple.Create(1), Tuple.Create(4), Tuple.Create(3)};
        }

        [Test]
        public void Sort_arrays_ascending()
        {
            //when
            _unsorted.SortAscending(x => x.Item1);

            //then
            var expected = new[] {Tuple.Create(1), Tuple.Create(2), Tuple.Create(3), Tuple.Create(4)};
            
            CollectionAssert.AreEqual(expected, _unsorted);
        }

        [Test]
        public void Sort_arrays_descending()
        {
            //when
            _unsorted.SortDescending(x => x.Item1);

            //then
            var expected = new[] { Tuple.Create(4), Tuple.Create(3), Tuple.Create(2), Tuple.Create(1) };

            CollectionAssert.AreEqual(expected, _unsorted);
        }
    }
}