using AppBrix.Configuration;
using AppBrix.Configuration.Files;
using AppBrix.Configuration.Yaml;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Linq;

namespace AppBrix.WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebHost.CreateDefaultBuilder(args)
                .UseApp(App.Start<WebAppInitializerModule>(new ConfigService(
                    new FilesConfigProvider("./Config", "yaml"), new YamlConfigSerializer())))
                .Build()
                .Run();
        }
    }
}
