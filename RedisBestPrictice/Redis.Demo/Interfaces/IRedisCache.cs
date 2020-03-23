using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redis.Demo.Interfaces
{
    public interface IRedisCache
    {
        long ListLength(string key);
        void StringSet(string key, string value, TimeSpan? expiry = null);

        string StringGet(string key);

        long StringIncrement(string key, long value = 1);

        string StringGetRange(string key, long start, long end);

        long StringDecrement(string key, long value = 1);

        void HashSet(string key, HashEntry[] hashFields);

        string HashGet(string key, string hashField);


        RedisValue[] HashGet(string key, RedisValue[] hashFields);

        HashEntry[] HashGetAll(string key);

        bool KeyDelete(string key);

        IEnumerable<HashEntry> HashScan(string key, string pattern, int pageSize);
        void HashSet(string key, string hashField, string hashValue);

        long ListRightPush(string key, RedisValue[] values);

        RedisValue[] ListRange(string key, long start = 0, long end = -1);
        void ListTrim(string key, long start, long end);
        IDatabase Database { get;}
    }
}
