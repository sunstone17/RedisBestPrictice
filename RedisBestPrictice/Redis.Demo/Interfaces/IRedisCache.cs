using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redis.Demo.Interfaces
{
    public interface IRedisCache
    {
        void StringSet(string key, string value, TimeSpan? expiry = null);

        string StringGet(string key);

        long StringIncrement(string key, long value = 1);

        string StringGetRange(string key, long start, long end);

        long StringDecrement(string key, long value = 1);

        void HashSet(string key, HashEntry[] hashFields);

        void HashGet(string key, string hashField);


        RedisValue[] HashGet(string key, RedisValue[] hashFields);

        HashEntry[] HashGetAll(string key);
    }
}
