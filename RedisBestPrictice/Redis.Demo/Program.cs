using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Redis.Demo.Interfaces;
using Redis.Demo.Model;
using Redis.Demo.Services;
using System;

namespace Redis.Demo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", false, true)
                .Build();


            IServiceCollection services = new ServiceCollection();

            services.AddSingleton<IRedisCache, RedisCache>();
            services.AddSingleton<RedisConfigurationOptions>(x => new RedisConfigurationOptions
            {
                EndPoint = configuration.GetSection("RedisConfigurationOptions:EndPoint")?.Value,
                Password = configuration.GetSection("RedisConfigurationOptions:Password")?.Value
            });


            AutofacContainer.Build(services);

            var test = AutofacContainer.Resolve<RedisConfigurationOptions>();

            Console.WriteLine("Hello World!");

        }
    }
}
