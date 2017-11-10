using AppBrix.Application;
using System;
using System.Diagnostics;
using System.Linq;

namespace AppBrix.ConsoleApp
{
    /// <summary>
    /// This is a sample console application using an AppBrix app.
    /// </summary>
    internal sealed class Program
    {
        internal static void Main(string[] args)
        {
            var stopwatch = Stopwatch.StartNew();
            var app = ConsoleAppInitializerModule.CreateApp();
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
            var generatorKey = typeof(MessageGenerator).FullName;
            app.GetFactory().Register(() => new MessageGenerator("Test"));

            var cache = app.GetMemoryCache();
            cache.Set(generatorKey, app.GetFactory().Get<MessageGenerator>());

            for (var i = 0; i < 20; i++)
            {
                var generator = cache.Get<MessageGenerator>(generatorKey);
                app.GetLogHub().Info(generator.Generate());
                cache.Set(generatorKey, generator);
            }

            cache.Remove(generatorKey);
        }
    }
}
