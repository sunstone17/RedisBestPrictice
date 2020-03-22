using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Redis.Demo.Interfaces;
using Redis.Demo.Services;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Redis.Demo.Tests
{
    internal class Person
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public int Age { get; set; }

        public bool? Sex { get; set; }

        public string Hobby { get; set; }
    }

    public class Hashs : TestBase
    {
        private Person[] Users = new Person[2] {
            new Person{
                Id = Me(),
                Age = 30,
                Sex = false,
                Hobby = "ppppingg",
                Name = "dashan"
            },
            new Person{
                Id = Me(),
                Age = 30,
                Hobby = "bannnng",
                Name = "xiaoshui"
            },
        };

        [Test]
        public void TestScanHashs()
        {
            var redisCache = AutofacContainer.Resolve<IRedisCache>();

            var hashKey = Me();

            for (int i = 0; i < 200; i++)
            {
                redisCache.HashSet(hashKey, "key" + i, "value " + i);
            }
            var count = 0;

            foreach (var item in redisCache.HashScan(hashKey, null, pageSize: 20))
            {
                count++;
            }

            Assert.AreEqual(count, 200);
        }

        [Test]
        public void TestHMSet()
        {
            var redisCache = AutofacContainer.Resolve<IRedisCache>();

            var arr = Users;

            foreach (var user in arr)
            {
                var data = new List<HashEntry> {
                    new HashEntry(nameof(Person.Id), user.Id),
                    new HashEntry(nameof(Person.Name), user.Name),
                    new HashEntry(nameof(Person.Hobby), user.Hobby),
                    new HashEntry(nameof(Person.Age), user.Age),
                };

                //不能为Null
                if (user.Sex.HasValue)
                {
                    data.Add(new HashEntry(nameof(Person.Sex), user.Sex));
                }

                redisCache.HashSet(user.Id, data.ToArray());

                var result = redisCache.HashGetAll(user.Id);

                Assert.NotNull(result.ToList().First(p => p.Name == nameof(Person.Id)));

                JObject jObject = new JObject();
                foreach (HashEntry entry in result)
                {
                    var jValue = new JValue(entry.Value);
                    jObject[entry.Name] = jValue;
                }

                if (jObject.ContainsKey(nameof(Person.Sex)))
                {
                    jObject[nameof(Person.Sex)] = jObject[nameof(Person.Sex)] == null ? jObject[nameof(Person.Sex)] : (int)jObject[nameof(Person.Sex)];
                }

                var currentUser = jObject.ToObject<Person>();
            }
        }

        [Test]
        public void TestHSet()
        {
            var redisCache = AutofacContainer.Resolve<IRedisCache>();

            var user = Users[0];


            var data = new List<HashEntry> {
                    new HashEntry(nameof(Person.Id), user.Id),
                    new HashEntry(nameof(Person.Name), user.Name),
                    new HashEntry(nameof(Person.Hobby), user.Hobby),
                    new HashEntry(nameof(Person.Age), user.Age),
                };

            //不能为Null
            if (user.Sex.HasValue)
            {
                data.Add(new HashEntry(nameof(Person.Sex), user.Sex));
            }

            redisCache.HashSet(user.Id, data.ToArray());

            var result = redisCache.HashGetAll(user.Id);

            Assert.NotNull(result.ToList().First(p => p.Name == nameof(Person.Id)));

            redisCache.HashSet(user.Id, nameof(Person.Age), "100");

            Assert.AreEqual("100", redisCache.HashGet(user.Id, nameof(Person.Age)));
        }

    }
}
