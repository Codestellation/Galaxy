using System;
using Codestellation.Quarks.DateAndTime;
using NUnit.Framework;

namespace Codestellation.Quarks.Tests.DateAndTime
{
    public class DateTimeExtensionsTests
    {
        [Test]
        public void Should_discard_milliseconds()
        {
            var dateTime = new DateTime(2014, 12, 1, 12, 23, 45, 28);

            var result = dateTime.DiscardMilliseconds();

            var expected = new DateTime(2014, 12, 1, 12, 23, 45);

            Assert.That(result, Is.EqualTo(expected));
        } 
    }
}