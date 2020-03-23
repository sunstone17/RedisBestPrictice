using NUnit.Framework;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redis.Demo.Tests
{
    public class SortedSets : TestBase
    {
        public static SortedSetEntry[] entries = new SortedSetEntry[]
        {

            new SortedSetEntry("a", 1),
            new SortedSetEntry("b", 2),
            new SortedSetEntry("c", 3),
            new SortedSetEntry("d", 4),
            new SortedSetEntry("f", 6),
            new SortedSetEntry("e", 5),
            new SortedSetEntry("g", 7),
            new SortedSetEntry("h", 8),
            new SortedSetEntry("i", 9),
            new SortedSetEntry("j", 10)
        };

        [Test]
        public void SortedSetPop()
        {
            var db = RedisCache.Database;
            var key = Me();

            db.KeyDelete(key, CommandFlags.FireAndForget);

            db.SortedSetAdd(key, entries);

            var first = db.SortedSetPop(key, Order.Ascending);
            Assert.True(first.HasValue);
            Assert.Equals(first.Value, entries[0]);
            Assert.AreEqual(9, db.SortedSetLength(key));

            var lasts = db.SortedSetPop(key, 2, Order.Descending);
            Assert.AreEqual(2, lasts.Length);
            Assert.AreEqual(entries[9], lasts[0]);
            Assert.AreEqual(entries[8], lasts[1]);
            Assert.AreEqual(7, db.SortedSetLength(key));
        }
    }
}
