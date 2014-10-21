using System;
using Codestellation.Quarks.Enumerations;
using NUnit.Framework;

namespace Codestellation.Quarks.Tests.Enumerations
{
    [TestFixture]
    public class EnumIndexerTests
    {
        public enum YesNo : byte
        {
            Yes,
            No
        }
        
        [Test]
        public void Throws_if_key_is_no_enum()
        {
            Assert.Throws<TypeInitializationException>(() => new EnumIndexer<DateTime, Boolean>());
        }

        [Test]
        public void Can_set_and_get_value_at_index()
        {
            var indexer = new EnumIndexer<YesNo, DateTime>();

            indexer[YesNo.No] = DateTime.MaxValue;

            var result = indexer[YesNo.No];

            Assert.That(result, Is.EqualTo(DateTime.MaxValue));
        }
    }
}