using System;
using System.Threading;
using Codestellation.Quarks.Threading;
using NUnit.Framework;

namespace Codestellation.Quarks.Tests.Threading
{
    [TestFixture]
    public class PriciseTimerTests
    {
        [Test]
        public void Should_fire_once()
        {
            int fireCount = 0;
            var timer = new PreciseTimer(state => fireCount++ , null);
            timer.FireAt(DateTime.Now.AddMilliseconds(100));

            Thread.Sleep(1000);
            timer.Dispose();

            Assert.That(fireCount, Is.EqualTo(1));
        }


        [Test]
        public void Should_fire_multiple_times()
        {
            int fireCount = 0;
            var timer = new PreciseTimer(state =>
            {
                if (fireCount < 10)
                {
                    fireCount++;
                }
            }, null);

            timer.FireAndRepeat(DateTime.Now, TimeSpan.FromMilliseconds(100));

            Thread.Sleep(1000);
            timer.Dispose();

            Assert.That(fireCount, Is.EqualTo(10));
        }
    }
}