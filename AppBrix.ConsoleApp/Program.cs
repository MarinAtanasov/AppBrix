using AppBrix.Application;
using AppBrix.Configuration;
using AppBrix.Configuration.Files;
using AppBrix.Configuration.Json;
using System;
using System.Diagnostics;
using System.Linq;

namespace AppBrix.ConsoleApp
{
    /// <summary>
    /// This is a sample console application using an AppBrix app.
    /// </summary>
    internal class Program
    {
        internal static void Main(string[] args)
        {
            // Required when using the web client and receiving windows-1251 result.
            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var stopwatch = Stopwatch.StartNew();
            var configManager = new ConfigManager(new FilesConfigProvider("./Config", "json"), new JsonConfigSerializer());
            if (configManager.Get<AppConfig>().Modules.Count == 0)
                configManager.Get<AppConfig>().Modules.Add(ModuleConfigElement.Create<ConfigInitializerModule>());

            var app = App.Create(configManager);
            app.Start();
            try
            {
                Program.Run(app);
            }
            catch (Exception ex)
            {
                app.GetLog().Error("The application has stopped because of an error!", ex);
            }
            finally
            {
                app.Stop();
                Console.WriteLine("Executed in: {0} seconds.", stopwatch.Elapsed.TotalSeconds);
            }
        }

        private static void Run(IApp app)
        {
            var generatorKey = typeof(MessageGenerator).FullName;
            app.GetFactory().Register(() => new MessageGenerator("Test"));

            var cache = app.GetCache();
            cache.Set(generatorKey, app.GetFactory().Get<MessageGenerator>());

            for (var i = 0; i < 20; i++)
            {
                var generator = cache.Get<MessageGenerator>(generatorKey).Result;
                app.GetLog().Info(generator.Generate());
                cache.Set(generatorKey, generator);
            }

            cache.Remove(generatorKey);
        }
    }
}
