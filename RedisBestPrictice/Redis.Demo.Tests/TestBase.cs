﻿using NUnit.Framework;
using Redis.Demo.Interfaces;
using Redis.Demo.Services;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Redis.Demo.Tests
{
    public abstract class TestBase
    {
        [SetUp]
        public void Setup()
        {
            TestProgram.Main(null);
        }

        public static string Me(string filePath = null, string caller = null) =>
    "dotnetcore31-" + Path.GetRandomFileName() + "-" + caller;

        public IRedisCache RedisCache => AutofacContainer.Resolve<IRedisCache>();


        protected IServer GetAnyMaster(IConnectionMultiplexer muxer)
        {
            foreach (var endpoint in muxer.GetEndPoints())
            {
                var server = muxer.GetServer(endpoint);
                if (!server.IsSlave) return server;
            }
            throw new InvalidOperationException("Requires a master endpoint (found none)");
        }
    }
}
