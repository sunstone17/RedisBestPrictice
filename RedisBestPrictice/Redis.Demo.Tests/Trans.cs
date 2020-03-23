using NUnit.Framework;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Redis.Demo.Tests
{
    public class Trans : TestBase
    {
        [Test]
        public async Task BasicTranWithHashExistsCondition()
        {
            var db = RedisCache.Database;

            var key = Me();
            var key2 = Me() + "2";
            var hashField = "field";

            await db.HashSetAsync(key2, hashField, "any value");

            var tran = db.CreateTransaction();
            tran.AddCondition(Condition.HashExists(key2, hashField));
            var inc = tran.StringIncrementAsync(key);
            //await tran.StringIncrementAsync(key);//错误的写法
            var exec = await tran.ExecuteAsync();

            var get = db.StringGet(key);

            Assert.IsTrue(exec);

            Assert.AreEqual(1, (int)get);
            Assert.AreEqual(1, await inc);
        }
    }
}
