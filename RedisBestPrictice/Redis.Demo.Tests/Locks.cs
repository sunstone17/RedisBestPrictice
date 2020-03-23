using NUnit.Framework;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Redis.Demo.Tests
{
    public class Locks : TestBase
    {
        internal string Key = Guid.NewGuid().ToString();
        internal string Value = Guid.NewGuid().ToString();
        private bool AcqureLock(string key, string value, TimeSpan timeSpan)
        {
            var db = RedisCache.Database;
            bool flag = false;
            try
            {
                flag = db.StringSet(key, value, timeSpan, When.NotExists);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return flag;
        }


        /// <summary>
        /// 单点Redis的申请锁，释放锁
        /// </summary>
        public void TestLockV2()
        {
            var expiration = TimeSpan.FromSeconds(5);
            var db = RedisCache.Database;

            Parallel.For(0, 5, x =>
            {
                string person = $"Person: {x}";

                bool isLock = db.LockTake(Key, person, expiration);

                var val = 0;
                while (!isLock && val <= 5000)
                {
                    val += 250;
                    System.Threading.Thread.Sleep(250);
                    Console.WriteLine($"{person} is waiting, {DateTimeOffset.Now.ToUnixTimeMilliseconds()}");
                    isLock = db.LockTake(Key, person, expiration);
                }

                if (isLock)
                {
                    Console.WriteLine($"{person} got the lock at {DateTimeOffset.Now.ToUnixTimeMilliseconds()}");

                    if (new Random().NextDouble() < 10)
                    {
                        Console.WriteLine($"{person} release lock {db.LockRelease(Key, person)}  {DateTimeOffset.Now.ToUnixTimeMilliseconds()}");
                    }
                    else
                    {
                        Console.WriteLine($"{person} do not release lock ....");
                    }
                }
                else
                {
                    Console.WriteLine($"{person} die {DateTimeOffset.Now.ToUnixTimeMilliseconds()}");
                }
            });
        }


        /// <summary>
        /// 单点Redis的申请锁，释放锁
        /// </summary>
        [Test]
        public void TestLock()
        {
            var expiration = TimeSpan.FromSeconds(5);
            Parallel.For(0, 5, x =>
            {
                string person = $"Person: {x}";

                bool isLock = AcqureLock(Key, person, expiration);

                var val = 0;
                while (!isLock && val <= 5000)
                {
                    val += 250;
                    System.Threading.Thread.Sleep(250);
                    Console.WriteLine($"{person} is waiting, {DateTimeOffset.Now.ToUnixTimeMilliseconds()}");
                    isLock = AcqureLock(Key, person, expiration);
                }

                if (isLock)
                {
                    Console.WriteLine($"{person} got the lock at {DateTimeOffset.Now.ToUnixTimeMilliseconds()}");

                    if (new Random().NextDouble() < 10)
                    {
                        Console.WriteLine($"{person} release lock {ReleaseLock(Key, person)}  {DateTimeOffset.Now.ToUnixTimeMilliseconds()}");
                    }
                    else
                    {
                        Console.WriteLine($"{person} do not release lock ....");
                    }
                }
                else
                {
                    Console.WriteLine($"{person} die {DateTimeOffset.Now.ToUnixTimeMilliseconds()}");
                }
            });
        }


        bool ReleaseLock(string key, string value)
        {
            string lua_script = @"  
    if (redis.call('GET', KEYS[1]) == ARGV[1]) then  
        redis.call('DEL', KEYS[1])  
        return true  
    else  
        return false  
    end  
    ";

            try
            {
                var res = RedisCache.Database.ScriptEvaluate(lua_script,
                                                           new RedisKey[] { key },
                                                           new RedisValue[] { value });
                return (bool)res;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ReleaseLock lock fail...{ex.Message}");
                return false;
            }
        }
        /*
        1. 那么为什么要设置key过期呢？
            如果请求执行因为某些原因意外退出了，导致创建了锁但是没有删除锁，那么这个锁将一直存在，以至于以后缓存再也得不到更新。
            使用SETNX带有过期时间的锁
        2. redis发现锁失败了要怎么办？中断请求还是循环请求？ 
            循环获取锁
        3. 循环请求的话，如果有一个获取了锁，其它的在去获取锁的时候，是不是容易发生抢锁的可能？
            在循环请求获取锁的时候，加入睡眠功能，等待几毫秒在执行循环
        4. 锁提前过期后，客户端A还没执行完，然后客户端B获取到了锁，这时候客户端A执行完了，会不会在删锁的时候把B的锁给删掉？
            在删除key的时候判断下存入的key里的value和自己存的是否一样
            StackExchange.Redis 中实现见 GetLockReleaseTransaction 方法
         */
        private ITransaction GetLockReleaseTransaction(RedisKey key, RedisValue value)
        {
            var tran = default(ITransaction);
            if (tran != null)
            {
                tran.AddCondition(Condition.StringEqual(key, value));
                tran.KeyDeleteAsync(key, CommandFlags.FireAndForget);
            }
            return tran;
        }
    }
}

