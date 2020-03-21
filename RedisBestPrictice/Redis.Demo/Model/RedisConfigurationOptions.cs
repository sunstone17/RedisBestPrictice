using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redis.Demo.Model
{
    public class RedisConfigurationOptions
    {
        public ConfigurationOptions ConfigurationOptions { get; set; }
        public string EndPoint { get; set; }
        public string Password { get; set; }
    }
}
