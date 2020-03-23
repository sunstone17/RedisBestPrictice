using NUnit.Framework;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Redis.Demo.Tests
{
    public class Keys : TestBase
    {
        [Test]
        public void IdleTime()
        {
            var db = RedisCache.Database;

            var key = Me();

            db.StringSet(key, "new value",
                flags: CommandFlags.FireAndForget);

            Task.Delay(2000).Wait();

            var idleTime = db.KeyIdleTime(key);

            Assert.True(idleTime > TimeSpan.Zero);
        }
    }
}
