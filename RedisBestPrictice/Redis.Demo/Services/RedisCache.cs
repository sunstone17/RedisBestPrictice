using Redis.Demo.Interfaces;
using Redis.Demo.Model;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redis.Demo.Services
{
    public class RedisCache : IRedisCache
    {
        public RedisCache()
        {

        }

        public ConnectionMultiplexer _redisManager { get; set; }
        public IDatabase Database
        {
            get
            {
                return Manager.GetDatabase();
            }
        }

        public ConnectionMultiplexer Manager
        {
            get
            {
                if (_redisManager == null)
                {
                    _redisManager = GetManager();
                }
                return _redisManager;
            }
        }

        ConnectionMultiplexer GetManager()
        {
            if (_redisManager == null)
            {
                var options = AutofacContainer.Resolve<RedisConfigurationOptions>();
                _redisManager = ConnectionMultiplexer.Connect(options.EndPoint);
            }
            return _redisManager;
        }

        public void StringSet(string key, string value, TimeSpan? expiry = null)
        {
            Database.StringSet(key, value, expiry);
        }

        public string StringGet(string key)
        {
            return Database.StringGet(key);
        }

        public string StringGetRange(string key, long start, long end)
        {
            return Database.StringGetRange(key, start, end);
        }

        public long StringIncrement(string key, long value = 1)
        {
            return Database.StringIncrement(key, value);
        }

        public long StringDecrement(string key, long value = 1)
        {
            return Database.StringDecrement(key, value);
        }

        public void HashSet(string key, string hashField, string hashValue)
        {
            Database.HashSet(key, hashField, hashValue);
        }

        public void HashSet(string key, HashEntry[] hashFields)
        {
            Database.HashSet(key, hashFields);
        }

        public string HashGet(string key, string hashField)
        {
            return Database.HashGet(key, hashField);
        }
        public RedisValue[] HashGet(string key, RedisValue[] hashFields)
        {
            return Database.HashGet(key, hashFields);
        }

        public HashEntry[] HashGetAll(string key)
        {
            return Database.HashGetAll(key);
        }

        public bool KeyDelete(string key)
        {
            return Database.KeyDelete(key);
        }

        public IEnumerable<HashEntry> HashScan(string key, string pattern, int pageSize)
        {
            return Database.HashScan(key, pattern, pageSize);
        }

        public long ListRightPush(string key, RedisValue[] values)
        {
            return Database.ListRightPush(key, values);
        }

        public long ListLength(string key)
        {
            return Database.ListLength(key);
        }

        public RedisValue[] ListRange(string key, long start = 0, long end = -1)
        {
            return Database.ListRange(key, start, end);
        }
        public void ListTrim(string key, long start, long end)
        {
            Database.ListTrim(key, start, end);
        }
    }
}