using NUnit.Framework;
using Redis.Demo.Interfaces;
using Redis.Demo.Services;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redis.Demo.Tests
{
    public class Lists : TestBase
    {
        [Test]
        public void ListTest()
        {
            var redis = RedisCache;

            var key = Me();

            var data = "abcdefghijklmnopqrstuvwxyz".Select(p => (RedisValue)p.ToString()).ToArray();

            redis.ListRightPush(key, data);

            Assert.AreEqual(26, redis.ListLength(key));

            redis.ListTrim(key, 0, -11);

            Assert.AreEqual(16, redis.ListLength(key));

            Assert.AreEqual("abcdefghijklmnop", string.Concat(redis.ListRange(key)));//支持分片
        }
    }
}
