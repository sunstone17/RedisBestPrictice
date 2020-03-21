using NUnit.Framework;
using Redis.Demo.Interfaces;
using Redis.Demo.Services;
using System;
using System.Threading;

namespace Redis.Demo.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            Program.Main(null);
        }

        [Test]
        public void TestString()
        {
            var redisCache = AutofacContainer.Resolve<IRedisCache>();

            var key1 = "abc";
            var value1 = "testabc";
            redisCache.StringSet(key1, value1);
            Assert.AreEqual(value1, redisCache.StringGet(key1));

            Assert.AreEqual(value1.Substring(1, 3), redisCache.StringGetRange(key1, 1, 3));

            redisCache.StringSet(key1, value1, new TimeSpan(0, 0, 2));
            Thread.Sleep(3 * 1000);
            Assert.AreEqual(null, redisCache.StringGet(key1));
        }
    }
}