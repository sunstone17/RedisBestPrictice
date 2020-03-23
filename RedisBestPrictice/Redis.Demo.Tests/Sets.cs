using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redis.Demo.Tests
{
    public class Sets : TestBase
    {
        [Test]
        public void SetPop()
        {
            var db = RedisCache.Database;

            var key = Me();

            db.KeyDelete(key, StackExchange.Redis.CommandFlags.FireAndForget);

            for (int i = 1; i < 11; i++)
            {
                db.SetAdd(key, i);
            }

            var random = db.SetPop(key);
            Assert.False(random.IsNull);
            Assert.True((int)random <= 10);

            var moreRandoms = db.SetPop(key, 2);

            Assert.AreEqual(2, moreRandoms.Length);
            Assert.False(moreRandoms[0].IsNull);
            Assert.AreEqual(7, db.SetLength(key));
        }

        [Test]
        public void SScan()
        {
            var db = RedisCache.Database;

            var key = Me();

            var totalFilter = 0;
            for (int i = 1; i < 1000; i++)
            {
                db.SetAdd(key, i);
                if (i.ToString().Contains("3"))
                    totalFilter += i;
            }

            var filter = db.SetScan(key, "*3*").Select(x => (int)x).Sum();

            Assert.AreEqual(filter, totalFilter);
        }

        [Test]
        public void SetAdd()
        {
            var db = RedisCache.Database;

            var key = Me();

            for (int i = 1; i < 10; i++)
            {
                db.SetAdd(key, 2);
            }

            foreach(var t in db.SetScan(key, "*2*"))
            {
                Console.WriteLine(t);
            }
        }
    }
}
