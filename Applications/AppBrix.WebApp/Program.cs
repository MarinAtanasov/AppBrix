using AppBrix.Configuration;
using AppBrix.Configuration.Files;
using AppBrix.Configuration.Yaml;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace AppBrix.WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Host.CreateDefaultBuilder(args)
                .UseWebApp(App.Start<WebAppMainModule>(new ConfigService(
                    new FilesConfigProvider("./Config", "yaml"), new YamlConfigSerializer())))
                .Build()
                .Run();
        }
    }
}
