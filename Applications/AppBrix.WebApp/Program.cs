using AppBrix.Configuration;
using AppBrix.Configuration.Files;
using AppBrix.Configuration.Yaml;
using Microsoft.AspNetCore.Builder;

namespace AppBrix.WebApp;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplication.CreateBuilder(args)
            .Build(App.Start<WebAppMainModule>(new ConfigService(
                new FilesConfigProvider("./Config", "yaml"), new YamlConfigSerializer())))
            .Run();
    }
}
