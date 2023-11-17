using AppBrix.Configuration;
using AppBrix.Configuration.Files;
using AppBrix.Configuration.Json;
using System;
using System.Diagnostics;

namespace AppBrix.ConsoleApp;

/// <summary>
/// This is a sample console application using an AppBrix app.
/// </summary>
internal static class Program
{
    internal static void Main()
    {
        var stopwatch = Stopwatch.StartNew();
        var app = App.Start<MainModule>(new ConfigService(new FilesConfigProvider(
            new JsonConfigSerializer(), "./Config", "json")));
        try
        {
            Program.Run(app);
        }
        catch (Exception ex)
        {
            app.GetLogHub().Error("The application has stopped because of an error!", ex);
        }
        finally
        {
            app.Stop();
            Console.WriteLine("Executed in: {0} seconds.", stopwatch.Elapsed.TotalSeconds);
        }
    }

    private static void Run(IApp app)
    {
        var generatorKey = typeof(MessageGenerator).FullName!;
        app.GetFactoryService().Register(() => new MessageGenerator("Test"));

        var cache = app.GetMemoryCache();
        cache.Set(generatorKey, app.GetFactoryService().Get<MessageGenerator>());

        for (var i = 0; i < 20; i++)
        {
            var generator = cache.Get<MessageGenerator>(generatorKey);
            app.GetLogHub().Info(generator.Generate());
            cache.Set(generatorKey, generator);
        }

        cache.Remove(generatorKey);
    }
}
