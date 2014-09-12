using System;
using Codestellation.Quarks.DateAndTime;
using NUnit.Framework;

namespace Codestellation.Quarks.Tests.DateAndTime
{
    [TestFixture]
    public class ClockTests
    {
        [SetUp]
        public void SetUp()
        {
            Clock.SetRealTime();
        }

        [Test]
        public void Should_throw_if_date_time_kind_is_unspecified()
        {
            var undefined = new DateTime(2010, 11, 12, 12, 20, 43, DateTimeKind.Unspecified);

            Assert.Throws<ArgumentException>(()=> Clock.FixClockAt(undefined));
        }

        [Test]
        public void Should_return_fixed_time_when_specified()
        {
            var fixedTime = new DateTime(2010, 11, 12, 12, 20, 43, DateTimeKind.Utc);

            Clock.FixClockAt(fixedTime);

            Assert.That(Clock.UtcNow, Is.EqualTo(fixedTime));
        }
        
        [Test]
        public void Should_restore_fixed_time_when_specified()
        {
            //given
            var fixedTime = new DateTime(2010, 11, 12, 12, 20, 43, DateTimeKind.Utc);
            Clock.FixClockAt(fixedTime);

            //when
            Clock.SetRealTime();

            //then
            var now = DateTime.UtcNow;
            var diff = now - Clock.UtcNow;

            //Non-strict comparison due to possible 
            Assert.That(diff, Is.LessThan(TimeSpan.FromMinutes(1)));
        }
    }
}