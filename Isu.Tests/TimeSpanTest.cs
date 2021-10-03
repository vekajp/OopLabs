using System;
using Isu.TimeEntities;
using NUnit.Framework;
using TimeSpan = Isu.TimeEntities.TimeSpan;

namespace Isu.Tests
{
    public class TimeSpanTest
    {
        [Test]
        public void TestIntersects()
        {
            TimeSpan span1 = new TimeSpan(new Time(8, 10), new Time(9, 50));
            TimeSpan span2 = new TimeSpan(new Time(9, 0), new Time(10, 50));
            TimeSpan span3 = new TimeSpan(new Time(11, 10), new Time(12, 50));
            
            Assert.That(span1.Intersects(span2));
            Assert.That(span2.Intersects(span1));
            Assert.That(!span1.Intersects(span3));
            Assert.That(!span3.Intersects(span1));
            Assert.That(!span2.Intersects(span3));
            Assert.That(span1.Intersects(span1));
        }
    }
}