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
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            var configService = new ConfigService(new FilesConfigProvider("./Config", "yaml"), new YamlConfigSerializer());
            if (configService.Get<AppConfig>().Modules.Count == 0)
                configService.Get<AppConfig>().Modules.Add(ModuleConfigElement.Create<ConfigInitializerModule>());

            var app = AppBrix.App.Create(configService);
            app.Start();

            return WebHost.CreateDefaultBuilder(args)
                .ConfigureServices(services => services.AddApp(app))
                .ConfigureLogging(logging => logging.AddProvider(app))
                .UseStartup<Startup>()
                .Build();
        }
    }
}
