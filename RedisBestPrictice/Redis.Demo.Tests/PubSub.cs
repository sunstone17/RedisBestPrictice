using NUnit.Framework;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Redis.Demo.Tests
{
    public class PubSub : TestBase
    {
        [Test]
        public async Task PubSubTest()
        {
            var manager = RedisCache.Manager;
            var key = Guid.NewGuid().ToString();
            HashSet<string> received = new HashSet<string>();

            var sub = manager.GetSubscriber();

            sub.Subscribe(key, (channel, payload) =>
            {
                lock (received)
                {
                    if (channel == key)
                    {
                        received.Add(payload);
                    }
                }
            }, CommandFlags.FireAndForget);

            var tasks = new List<Task>(10);

            foreach (var c in "asdfgh")
            {
                var task = new Task(() =>
                {
                    Thread.Sleep(500);
                    sub.Publish(key, c.ToString());
                });
                task.Start();
                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());

            await PingAsync(manager, GetAnyMaster(manager), sub).ForAwait();

            Assert.AreEqual(6, received.Count);
        }


        private static async Task PingAsync(IConnectionMultiplexer muxer, IServer pub, ISubscriber sub, int times = 1)
        {
            while (times-- > 0)
            {
                // both use async because we want to drain the completion managers, and the only
                // way to prove that is to use TPL objects
                var t1 = sub.PingAsync();
                var t2 = pub.PingAsync();
                await Task.Delay(100).ForAwait(); // especially useful when testing any-order mode

                if (!Task.WaitAll(new[] { t1, t2 }, muxer.TimeoutMilliseconds * 2)) throw new TimeoutException();
            }
        }
    }

    public static class __
    {
        public static ConfiguredTaskAwaitable ForAwait(this Task task) => task.ConfigureAwait(false);
    }
}
